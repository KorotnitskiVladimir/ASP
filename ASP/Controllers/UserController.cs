using System.Text.Json;
using ASP.Data;
using ASP.Models.User;
using Microsoft.AspNetCore.Mvc;

namespace ASP.Controllers;

public class UserController: Controller
{
    private const string signupFormKey = "UserSignupFormModel";

    private readonly DataContext _dataContext;
    // GET
    public UserController(DataContext dataContext)
    {
        _dataContext = dataContext;
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
                string salt = "salt";
                _dataContext.UserAccesses.Add(new()
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Login = viewModel.FormModel!.UserLogin,
                    RoleId = "guest",
                    Salt = salt,
                    Dk = salt + viewModel.FormModel!.UserPassword
                });
                _dataContext.SaveChanges();
            }
            HttpContext.Session.Remove(signupFormKey);
        }
        return View(viewModel);
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