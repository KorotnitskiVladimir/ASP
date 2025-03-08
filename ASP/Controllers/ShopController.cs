using ASP.Data;
using ASP.Models.Shop;
using ASP.Services.Storage;
using Microsoft.AspNetCore.Mvc;

namespace ASP.Controllers;

public class ShopController : Controller
{
    private readonly DataContext _dataContext;
    private readonly IStorageService _storageService;

    public ShopController(DataContext dataContext, IStorageService storageService)
    {
        _dataContext = dataContext;
        _storageService = storageService;
    }
    // GET
    public IActionResult Index()
    {
        ShopIndexViewModel viewModel = new()
        {
            Categories = _dataContext.Categories.ToList()
        };
        return View(viewModel);
    }

    public FileResult Image([FromRoute] string id)
    {
        return File(System.IO.File.ReadAllBytes(_storageService.GetRealPath(id)), "image/jpeg");
    }
}