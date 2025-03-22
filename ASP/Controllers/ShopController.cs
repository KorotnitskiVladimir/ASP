using System.Security.Claims;
using ASP.Data;
using ASP.Data.Entities;
using ASP.Models.Shop;
using ASP.Services.Storage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Syncfusion.EJ2.Notifications;

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

    public ViewResult Product([FromRoute] string id)
    {
        string controllerName = ControllerContext.ActionDescriptor.ControllerName;
        ShopProductViewModel viewModel = new()
        {
            Product = _dataContext
                .Products
                .Include(p => p.Category)
                .FirstOrDefault(p => p.Slug == id || p.Id.ToString() == id),
            BreadCrumbs = new()
            {
                new BreadCrumb() {Title = "Home page", Url = "/"},
                new BreadCrumb() {Title = "Shop", Url = $"/{controllerName}"},
            }
        };
        if (viewModel.Product != null)
        {
            viewModel.BreadCrumbs.Add(
                new BreadCrumb()
                {
                    Title = viewModel.Product.Category.Name,
                    Url = $"/{controllerName}/{nameof(Category)}/{viewModel.Product.Category.Slug}"
                });
            viewModel.BreadCrumbs.Add(
                new BreadCrumb()
                {
                    Title = viewModel.Product.Name,
                    Url = $"/{controllerName}/{nameof(Product)}/{viewModel.Product.Slug ?? viewModel.Product.Id.ToString()}"
                });
        }
        return View(viewModel);
    }

    [HttpPost]
    public JsonResult AddToCart([FromForm] string productId, [FromForm] string uaId)
    {
        Guid pIdVal;
        Guid uAVal;
        bool productIdValidity = Guid.TryParse(productId, out pIdVal);
        bool userAccessValidity = Guid.TryParse(uaId, out uAVal);
        if (productIdValidity && userAccessValidity)
        {
            Product? product = _dataContext.Products.FirstOrDefault(p => p.Id.ToString() == productId);
            UserAccess? ua = _dataContext.UserAccesses.FirstOrDefault(u => u.Id.ToString() == uaId);
            if (product == null)
            {
                return Json(new { Status = 404, message = "product not found" });
            }
            else if (ua == null)
            {
                return Json(new { Status = 404, message = "user not found" });
            }

            /*
             * Проверяем есть ли у пользователя незакрытая корзина. если есть - дополняем ее, если нет - создаем новую.
             */
            Cart? cart = _dataContext.Carts.FirstOrDefault(c => c.UserAccessId.ToString() == uaId);
            if (cart == null)
            {
                cart = new Cart()
                {
                    Id = Guid.NewGuid(),
                    UserAccessId = Guid.Parse(uaId),
                    OpenAt = DateTime.Now
                };
                _dataContext.Carts.Add(cart);
            }

            // Тоже самое для CartItem
            CartItem? cartItem = _dataContext.CartItems
                .FirstOrDefault(ci => ci.CartId == cart.Id && ci.ProductId.ToString() == productId);
            if (cartItem != null)
            {
                cartItem.Quantity += 1;
                cartItem.Price += product.Price; // пересчет по акциям
            }
            else
            {
                cartItem = new()
                {
                    Id = Guid.NewGuid(),
                    CartId = cart.Id,
                    ProductId = product.Id,
                    Quantity = 1,
                    Price = product.Price // Пересчет по акциям
                };
                _dataContext.CartItems.Add(cartItem);
            }

            cart.Price += product.Price; // Тоже пересчет по акциям
            _dataContext.SaveChanges();

            return Json(new { Status = 200 });
        }
        else
        {
            return Json(new { status = 404, message = "warning! problem with input" });
        }
    }

    public ViewResult Cart()
    {
        string? uaId = HttpContext.User.Claims
            .FirstOrDefault(c => c.Type == ClaimTypes.Sid)?.Value;
        ShopCartViewModel viewModel = new()
        {
            Cart = _dataContext.Carts
                .Include(c => c.CartItems)
                .FirstOrDefault(c => c.UserAccessId.ToString() == uaId)
        };
        
        return View(viewModel);
    }
}

// Корзина потребителя (заказ товаров)
// [UserAccess]    [Cart]                 [CartItems]
// Id --------\---- CartId ---------\      Id
//             \--- UserAccessId     \ --- CartId
//                  OpenAt                 ProductId -------------- [Product]
//                  CloseAt                Quantity
//                  IsCanceled             Price
//                  Price                  ActionId --------------- [Actions]
//                  ActionId ---------------------------------------[Actions]