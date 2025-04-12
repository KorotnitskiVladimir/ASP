using System.Security.Claims;
using ASP.Data;
using ASP.Data.Entities;
using ASP.Migrations;
using ASP.Models.Shop;
using ASP.Services.Storage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Syncfusion.EJ2.Notifications;
using Cart = ASP.Data.Entities.Cart;
using Product = ASP.Data.Entities.Product;

namespace ASP.Controllers;

public class ShopController : Controller
{
    private readonly DataContext _dataContext;
    private readonly IStorageService _storageService;
    private readonly DataAccessor _dataAccessor;

    public ShopController(DataContext dataContext, IStorageService storageService, DataAccessor dataAccessor)
    {
        _dataContext = dataContext;
        _storageService = storageService;
        _dataAccessor = dataAccessor;
    }
    // GET
    public IActionResult Index()
    {
        ShopIndexViewModel viewModel = new()
        {
            Categories = _dataAccessor.AllCategories()
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
            Categories = _dataAccessor.AllCategories(),
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
        Cart? cart = _dataContext.Carts
            .Include(c => c.CartItems)
            .ThenInclude(ci => ci.Product)
            .ThenInclude(p => p.Category)
            .ThenInclude(c => c.Products)
            .FirstOrDefault(c => c.UserAccessId.ToString() == uaId);
        
        List<Category> categories = new();
        List<Product> allProducts = new();
        List<Data.Entities.Product> products = new();
        foreach (var cat in cart.CartItems.Select(ci => ci.Product.Category))
        {
            categories.Add(cat);
        }

        foreach (var cat in cart.CartItems.Select(ci => ci.Product.Category.Products))
        {
            foreach (var prod in cat)
            {
                allProducts.Add(prod);
                products.Add(prod);
            }
        }
        
        foreach (var prod in allProducts)
        {
            foreach (var c in cart.CartItems.Select(ci => ci.Product))
            {
                if (prod.Id.CompareTo(c.Id) == 0)
                {
                    products.Remove(prod);
                }
            }
        }
        ShopCartViewModel viewModel = new()
        {
            Cart = cart == null ? null : cart with
            {
                CartItems = cart.CartItems.Select(ci => ci with
                {
                    Product = ci.Product with
                    {
                        ImagesCsv = string.IsNullOrEmpty(ci.Product.ImagesCsv) ? 
                            "Shop/Image/no-image.jpg"
                            : ci.Product.ImagesCsv = String.Join(',', ci.Product.ImagesCsv
                                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                                .Select(img => "/Shop/Image/" + img))
                    }
                }).ToList()
            },
            Products = products,
        };
        
        return View(viewModel);
    }

    [HttpPut]
    public JsonResult ModifyCartItem([FromQuery] string cartId, [FromQuery] int delta)
    {
        if (delta == 0)
        {
            return Json(new { status = 400, message = "Invalid zero value" });
        }
        Guid cartGuid;
        try
        {
            cartGuid = Guid.Parse(cartId);
        }
        catch
        {
            return Json(new { status = 400, message = "Invalid cartId format: UUID expected" });
        }

        CartItem? cartItem = _dataContext.CartItems
            .Include(ci => ci.Product)
            .Include(ci => ci.Cart)
            .FirstOrDefault(ci => ci.Id == cartGuid);
        if (cartItem == null)
        {
            return Json(new { status = 404, message = "Item not found" });
        }

        int newQuantity = cartItem.Quantity + delta;
        if (newQuantity < 0)
        {
            return Json(new { status = 400, message = "Invalid delta: negative total quantity" });
        }
        if (newQuantity > cartItem.Product.Stock)
        {
            return Json(new { status = 422, message = "Delta too large: not enough product in stock" });
        }

        if (newQuantity == 0)
        {
            cartItem.Cart.Price -= cartItem.Price;
            _dataContext.CartItems.Remove(cartItem);
        }
        else
        {
            cartItem.Cart.Price += delta * cartItem.Product.Price; // + Акции
            cartItem.Price += delta * cartItem.Product.Price; // + Акции
            cartItem.Quantity = newQuantity;
        }

        _dataContext.SaveChanges();
        return Json(new { status = 200, message = "Quantity modified" });
    }

    [HttpPut]
    public JsonResult CartHandler([FromQuery] string cartId, [FromQuery] int choice)
    {
        Guid cartGuid;
        try
        {
            cartGuid = Guid.Parse(cartId);
        }
        catch
        {
            return Json(new { status = 400, message = "Invalid cartId format: UUID expected" });
        }
        Cart? cart = _dataContext.Carts
            .Include(c => c.CartItems)
            .FirstOrDefault(c => c.Id == cartGuid);
        var products = _dataContext.Products;
        var cartProducts = cart.CartItems;
        if (cart == null)
        {
            return Json(new { status = 404, message = "Cart not found" });
        }

        if (choice == 0)
        {
            cart.IsCanceled = 1;
            //_dataContext.Carts.Remove(cart);
            _dataContext.SaveChanges();
            return Json(new { status = 200, message = "Cart canceled" });
        }
        else
        {
            int count = 0;
            foreach (var product in products)
            {
                foreach (var item in cartProducts)
                {
                    if (product.Id.CompareTo(item.ProductId) == 0)
                    {
                        if (product.Stock >= item.Quantity)
                        {
                            count++;
                        }
                    }
                }
            }
            if (count < cart.CartItems.Count)
            {
                return Json(new
                    { status = 422, message = "Regret not enough goods in stock. Please review your cart" });
            }
            else
            {
                foreach (var product in products)
                {
                    foreach (var item in cartProducts)
                    {
                        if (product.Id.CompareTo(item.ProductId) == 0)
                        {
                            product.Stock -= item.Quantity;
                        }
                    }
                }
                // По-хорошему нужно было бы и очистить корзину после отправки, но не знаю как лучше сделать
                // Нужно выше добавлять проверки и в случае если корзина была отправлена создавать новую корзину.
                // такие же проверки для отмененных корзин. Можно сделать, но не в контексте ДЗ =)
                cart.IsCanceled = 0;
                cart.CloseAt = DateTime.Now;
                _dataContext.SaveChanges();
                return Json(new { status = 200, message = "Cart submitted" });
            }
        }
        //return Json(new { status = 200, message = "Cart submitted" });
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
/*
 * API - Application - Program Interface - интерфейс взаимодействия Программы с ее Приложениями
 * Программа - информационный "центр" системы, чаще всего - бекенд
 * Приложение (application) - самостоятельный модуль, программа, которая, для своей работы, обменивается данными с
 * Программой
 * [Дополнение (Plugin, Addon) - не самостоятельная программа]
 *
 * Интерфейс - набор правил и шаблонов, по которым осуществляется обмен данными
 *
 *                     API                 API
 *          OpenAPI    --      Program      --     Tests
 *                       /        |         \
 *      Web-Front               Mobile          Desktop
 *       (Site)               Android/iOS
 *
 *      Cart ---------> Clone       Cart --------> Clone
 *        |         /                 |             |
 *       CartItem  /                CartItem ----> Clone
 *          |     /                   |             |
 *         Product                  Product -----> Clone
 * 
 */