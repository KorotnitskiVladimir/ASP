using ASP.Data;
using ASP.Models.Shop;
using ASP.Services.Storage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
    
    public IActionResult Category([FromRoute] string id)
    {
        ShopCategoryViewModel viewModel = new()
        {
            Categories = _dataContext.Categories.ToList(),
            Category = _dataContext
                .Categories
                .Include(c => c.Products) // заполнение навигационных свойств
                .FirstOrDefault(c => c.Slug == id)
        };
        return View(viewModel);
    }
}