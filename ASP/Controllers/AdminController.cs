using ASP.Data;
using ASP.Data.Entities;
using ASP.Services.Storage;
using ASP.Models.Admin;
using ASP.Services.AzureStorage;
using Azure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace ASP.Controllers;

public class AdminController : Controller
{
    private readonly DataContext _dataContext;
    private readonly IStorageService _storageService;
    private readonly ICloudStorageService _cloudStorage;
    public AdminController(DataContext dataContext, IStorageService storageService, ICloudStorageService cloudStorage)
    {
        _dataContext = dataContext;
        _storageService = storageService;
        _cloudStorage = cloudStorage;
    }

    public IActionResult Index()
    {
        string? canCreate = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "CanCreate")?.Value;
        if (canCreate != "1")
        {
            Response.StatusCode = StatusCodes.Status403Forbidden;
            return NoContent();
        }

        AdminIndexViewModel viewModel = new()
        {
            Categories = _dataContext.Categories.ToList(),
        };
        
        return View(viewModel);
    }

    [HttpPost]
    public JsonResult AddCategory(CategoryFormModel formModel)
    {
        Dictionary<string, string> errors = ValidateCategoryFormModel(formModel);
        if (errors.Count == 0)
        {
            Category category = new()
            {
                Id = Guid.NewGuid(),
                ParentId = null,
                Name = formModel.Name,
                Description = formModel.Description,
                Slug = formModel.Slug,
                //ImageUrl = _storageService.SaveFile(formModel.Image)
                ImageUrl = _cloudStorage.SaveFile(formModel.Image)
            };
            _dataContext.Categories.Add(category);
            _dataContext.SaveChanges();
            return Json(formModel);
        }
        else
        {
            return Json(new { status = 401, message = errors.Values });
        }
    }
    
    [HttpPost]
    public JsonResult AddProduct(ProductFormModel formModel)
    {
        double price;
        try
        {
            price = double.Parse(formModel.Price, System.Globalization.CultureInfo.InvariantCulture);
        }
        catch
        {
            price = double.Parse(formModel.Price.Replace(',', '.'),
                System.Globalization.CultureInfo.InvariantCulture);
        }
        Dictionary<string, string> errors = ValidateProductFormModel(formModel);
        if (errors.Count == 0)
        {
            Product product = new()
            {
                Id = Guid.NewGuid(),
                CategoryId = formModel.CategoryId,
                Name = formModel.Name,
                Description = formModel.Description,
                Slug = formModel.Slug,
                Price = price,
                Stock = formModel.Stock,
                ImagesCsv = string.Join(',', formModel.Images.Select(img => _storageService.SaveFile(img)))
            };
            _dataContext.Products.Add(product);
            try
            {
                _dataContext.SaveChanges();
            }
            catch
            {
                //_storageService.DeleteFile(product.ImagesCsv);
            }

            return Json(formModel);
        }
        else
        {
            return Json(new { status = 401, message = errors.Values });
        }
    }
    
    private Dictionary<string, string> ValidateCategoryFormModel(CategoryFormModel? formModel)
    {
        Dictionary<string, string> errors = new();
        if (formModel == null)
        {
            errors["Model"] = "Data not transferred";
        }
        else
        {
            if (string.IsNullOrEmpty(formModel.Name))
            {
                errors[nameof(formModel.Name)] = "Name is required";
            }
            if (string.IsNullOrEmpty(formModel.Description))
            {
                errors[nameof(formModel.Description)] = "Description is required";
            }
            if (string.IsNullOrEmpty(formModel.Slug))
            {
                errors[nameof(formModel.Slug)] = "Slug is required";
            }
            else
            {
                if (_dataContext
                        .Categories
                        .FirstOrDefault(c => c.Slug == formModel.Slug) != null)
                {
                    errors[nameof(formModel.Slug)] = "Such category exists already. Please choose new one";
                }
            }
            if (string.IsNullOrEmpty(formModel.Image.FileName))
            {
                errors[nameof(formModel.Image)] = "Image is required";
            }
        }
        return errors;
    }
    
    private Dictionary<string, string> ValidateProductFormModel(ProductFormModel? formModel)
    {
        double price;
        try
        {
            price = double.Parse(formModel.Price, System.Globalization.CultureInfo.InvariantCulture);
        }
        catch
        {
            price = double.Parse(formModel.Price.Replace(',', '.'),
                System.Globalization.CultureInfo.InvariantCulture);
        }
        Dictionary<string, string> errors = new();
        if (formModel == null)
        {
            errors["Model"] = "Data not transferred";
        }
        else
        {
            if (string.IsNullOrEmpty(formModel.Name))
            {
                errors[nameof(formModel.Name)] = "Name is required";
            }

            if (!string.IsNullOrEmpty(formModel.Slug))
            {
                if (_dataContext
                        .Products
                        .FirstOrDefault(c => c.Slug == formModel.Slug) != null)
                {
                    errors[nameof(formModel.Slug)] = "Product with such code exists already. Please choose new one";
                }

                if (string.IsNullOrEmpty(formModel.Images.ToString()))
                {
                    errors[nameof(formModel.Images)] = "Image(s) required";
                }
            }
            if (price <= 0)
            {
                errors[nameof(formModel.Price)] = "Price can't be less or equal to zero";
            }
            if (Convert.ToInt32(formModel.Stock) < 0)
            {
                errors[nameof(formModel.Stock)] = "Stock can't be less than zero";
            }
        }
        return errors;
    }
}