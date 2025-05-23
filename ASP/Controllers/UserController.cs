﻿using System.ComponentModel;
using System.Text.Json;
using ASP.Data;
using ASP.Data.Entities;
using ASP.Models.User;
using ASP.Services.KDF;
using ASP.Services.PasswordGenerator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace ASP.Controllers;

public class UserController: Controller
{
    private const string signupFormKey = "UserSignupFormModel";

    private readonly DataContext _dataContext;
    private readonly IKDFService _kdfService;
    private readonly DataAccessor _dataAccessor;
    private readonly IPasswordGeneratorService _passwordGeneratorService;
    // GET
    public UserController(DataContext dataContext, IKDFService kdfService, IPasswordGeneratorService passwordGeneratorService, DataAccessor dataAccessor)
    {
        _dataContext = dataContext;
        _kdfService = kdfService;
        _passwordGeneratorService = passwordGeneratorService;
        _dataAccessor = dataAccessor;
    }

    

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Signup()
    {
        UserSignupViewModel viewModel = new();
        if (HttpContext.Session.Keys.Contains(signupFormKey))
        {
            viewModel.FormModel = JsonSerializer.Deserialize<UserSignupFormModel>(
                HttpContext.Session.GetString(signupFormKey)!);
            viewModel.ValidationErrors = ValidateSignupFormModel(viewModel.FormModel);
            
            // Проверяем, если нет ошибок валидации, то регистрируем пользователя в БД
            if (viewModel.ValidationErrors.Count == 0)
            {
                Guid userId = Guid.NewGuid();
                _dataContext.UsersData.Add(new()
                {
                    Id = userId,
                    Name = viewModel.FormModel!.UserName,
                    Email = viewModel.FormModel!.UserEmail,
                    Phone = viewModel.FormModel!.UserPhone,
                    RegDate = DateTime.Now,
                    BirthDate = viewModel.FormModel!.BirthDate,
                    Social = viewModel.FormModel?.Social,
                    TorsoSize = viewModel.FormModel?.TorsoSize,
                    FootSize = viewModel.FormModel?.FootSize
                });
                string salt = _passwordGeneratorService.GeneratePassword(16);
                _dataContext.UserAccesses.Add(new()
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Login = viewModel.FormModel!.UserLogin,
                    RoleId = "guest",
                    Salt = salt,
                    Dk = _kdfService.DerivedKey(viewModel.FormModel!.UserPassword, salt)
                });
                _dataContext.SaveChanges();
            }
            HttpContext.Session.Remove(signupFormKey);
        }
        return View(viewModel);
    }

    public IActionResult Signin()
    {
        AccessToken accessToken;
        try
        {
            accessToken = _dataAccessor.Authenticate(Request);
        }
        catch (Win32Exception e)
        {
            return Json(new { status = e.ErrorCode, message = e.Message });
        }
        // Сохраняем в сессию вседения об аутентификации
        HttpContext.Session.SetString("userAccessId", accessToken.Sub.ToString());
        return Json(new { status = 200, message = "OK" });
    }

    public RedirectToActionResult Register([FromForm] UserSignupFormModel formModel)
    {
        HttpContext.Session.SetString(signupFormKey,  
            JsonSerializer.Serialize(formModel));
        return RedirectToAction(nameof(Signup));
    }

    private Dictionary<string, string> ValidateSignupFormModel(UserSignupFormModel? formModel)
    {
        Dictionary<string, string> errors =new();
        if (formModel == null)
        {
            errors["Model"] = "Data not transferred";
        }
        else
        {
            if (string.IsNullOrEmpty(formModel.UserName))
            {
                errors[nameof(formModel.UserName)] = "Name is required";
            }
            if (string.IsNullOrEmpty(formModel.UserEmail))
            {
                errors[nameof(formModel.UserEmail)] = "Email is required";
            }
            if (string.IsNullOrEmpty(formModel.UserPhone))
            {
                errors[nameof(formModel.UserPhone)] = "Phone number is required";
            }
            if (string.IsNullOrEmpty(formModel.UserLogin))
            {
                errors[nameof(formModel.UserLogin)] = "Login is required";
            }
            else
            {
                if (_dataContext
                        .UserAccesses
                        .FirstOrDefault(ua => ua.Login == formModel.UserLogin) != null)
                {
                    errors[nameof(formModel.UserLogin)] = "Such login exists already. Please choose new one";
                }
            }
            if (string.IsNullOrEmpty(formModel.UserPassword))
            {
                errors[nameof(formModel.UserPassword)] = "Password is required";
            }
            if (formModel.UserPassword != formModel.UserRepeat)
            {
                errors[nameof(formModel.UserRepeat)] = "Passwords do not match";
            }
            if (formModel.BirthDate == DateTime.MinValue)
            {
                errors[nameof(formModel.BirthDate)] = "Birth date is required";
            }
        }
        return errors;
    }
}

/*
 * Аутентификация - подтверждение личности, получение "удостоверения" (токена)
 * Авторизация - подтверждение права доступа уже аутентифицированной "особы" к определенному контенту
 * В зависимости от архитектуры системы токены сохраняются или:
 *  - у клиента (распределенная архитектура, SPA)
 *  - у сервера (серверная активность, ASP)
 *
 * По технологии ASP используются HTTP-сессии для сохранения данных между запросами. При каждом запросе проверяется
 * наличие в сессии токена и принимается решение относительно авторизации.
 *
 * <auth-form> --> site.js --X
 *                 fetch ---->   ASP
 *                   ?   <----  auth-status
 *                  + reload()
 *                  - error
 *
 * Base64 - кодирование с 64-мя символами
 *                                  A       B         C
 * ASCII(8 бит): ABC -->        01000001 01000010 01000011
 * делим по 6 бит:              010000 010100 001001 000011
 * определяем Base64-символы:      Q      U     J      D
 * код для "ABC" - "QUJD"
 * Если на 6 не делится, то используется символ выравнивания "="
 *    A       B
 * 01000001 01000010
 * 010000 010100 0010(00)
 *    Q     U       I =         --> QUI=
 *    A
 * 01000001
 * 010000 01(0000)
 *   Q      Q==                  -->QQ==         
 */
