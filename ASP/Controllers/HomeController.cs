using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ASP.Models;
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