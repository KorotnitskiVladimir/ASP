using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using ASP.Models;
using ASP.Models.Home;
using ASP.Services.PasswordGenerator;
using ASP.Services.Timestamp;

namespace ASP.Controllers;

public class HomeController : Controller
{
    // Инжекция - передача ссылок на службы, зарегистрированные в контейнере (см. Program.cs)
    // Пример инжекции - _logger, который идет "с коробки".
    private readonly ILogger<HomeController> _logger;
    // Для инжекции другого сервиса необходимо объявить переменную (желательно readonly)
    private readonly ITimestampService _timestampService;
    private readonly IPasswordGeneratorService _passwordGeneratorService;
    
    // Добавляем к параметрам конструктора зависимость от сервиса:
    public HomeController(ILogger<HomeController> logger, ITimestampService timestampService, IPasswordGeneratorService passwordGeneratorService)
    {
        _logger = logger;
        // сохраняем переданную ссылку для дальнейшего использования
        _timestampService = timestampService;
        _passwordGeneratorService = passwordGeneratorService;
    }

    public IActionResult Index()
    {
        return View();
    }
    
    public IActionResult Intro()
    {
        return View();
    }
    
    public IActionResult Razor()
    {
        return View();
    }
    
    public IActionResult IoC()
    {
        ViewData["timestamp"] = _timestampService.Timestamp;
        ViewData["timestampCode"] = _timestampService.GetHashCode();
        return View();
    }
    
    public IActionResult PasswordGenerator()
    {
        ViewData["passwordGenerator"] = _passwordGeneratorService.GeneratePassword(10);
        return View();
    }
    
    public IActionResult Models()
    {
        HomeModelsViewModel viewModel = new();
        // проверяем сохранены ли в сессии данные от формы Register
        if (HttpContext.Session.Keys.Contains("HomeModelsFormModel"))
        {
            // восстанавливаем объем модели из сериализованного состояния
            viewModel.FormModel = JsonSerializer.Deserialize<HomeModelsFormModel>(
                HttpContext.Session.GetString("HomeModelsFormModel")!);
            // удаляем из сессии извлеченные данные
            HttpContext.Session.Remove("HomeModelsFormModel");
        }
        return View(viewModel);
    }
    
    public IActionResult Register(HomeModelsFormModel formModel)
    {
        HttpContext.Session.SetString("HomeModelsFormModel",  // Сохранение сессии под ключем HomeModelsFormModel
            JsonSerializer.Serialize(formModel));             // сериализованного объекта formModel
        HomeModelsViewModel viewModel = new()
        {
            FormModel = formModel
        };
        return RedirectToAction(nameof(Models));
    }

    public JsonResult Ajax(HomeModelsFormModel formModel)
    {
        return Json(formModel);
    }

    public JsonResult AjaxJson([FromBody] HomeAjaxFormModel formModel)
    {
        return Json(formModel);
    }
    
    public IActionResult Review()
    {
        HomeModelsViewModel viewModel = new();
        if (HttpContext.Session.Keys.Contains("HomeModelReviewModel"))
        {
            viewModel.ReviewModel = JsonSerializer.Deserialize<HomeModelReviewModel>(
                HttpContext.Session.GetString("HomeModelReviewModel")!);
            HttpContext.Session.Remove("HomeModelReviewModel");
        }
        return View(viewModel);
    }
    
    public IActionResult ReviewForm(HomeModelReviewModel reviewModel)
    {
        HttpContext.Session.SetString("HomeModelReviewModel",  
            JsonSerializer.Serialize(reviewModel));             
        HomeModelsViewModel viewModel = new()
        {
            ReviewModel = reviewModel
        };
        return RedirectToAction(nameof(Review));
    }
    
    public JsonResult AjaxReview(HomeModelReviewModel reviewModel)
    {
        return Json(reviewModel);
    }

    public JsonResult AjaxJsonReview([FromBody] HomeAjaxReviewModel reviewModel)
    {
        return Json(reviewModel);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

/*
 * Browser                  Server(ASP)
 *
 * <form> ----------------->Register->View()            | Обновить страницу = повторить данный блок операций, 
 *                                         |            | в частности, отправить форму заново
 *  <------------------------------------HTML           |
 *
 * !! Формирование HTML на зпросы с данными (особенно формами) - требует пересмотра
 *
 *           POST  /Register
 * <form> ----------------->Register->X-->Сохранить данные до следующего запроса
 *                                    |                 | Для сохранения данных между запросами используется HTTP-сессия https://learn.microsoft.com/ru-ru/aspnet/core/fundamentals/app-state?view=aspnetcore-9.0
 *  <------------------------------Redirect /Models     |
 *  |
 *  |                       восстановить данные
 *  |     GET  /Models             |
 *  ----------------------->Models-->View()              | Обновление страницы - обновление GET /Models, которая не
 *                                        |              | передает данные
 *  <------------------------------------HTML            |
 *
 * -------------------------------- AJAX --------------------------------
 * <form>
 * submit -->X
 *          preventDefault()
 *              |               formData
 *            fetch---------------------------> Action()-->Json()
 *                              json                            |
 *            .then()<-------------------------------------------
*/