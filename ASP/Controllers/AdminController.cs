using ASP.Data;
using ASP.Data.Entities;
using ASP.Services.Storage;
using ASP.Views.Admin;
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
    public AdminController(DataContext dataContext, IStorageService storageService)
    {
        _dataContext = dataContext;
        _storageService = storageService;
    }

    public IActionResult Index()
    {
        string? canCreate = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "CanCreate")?.Value;
        if (canCreate != "1")
        {
            Response.StatusCode = StatusCodes.Status403Forbidden;
            return NoContent();
        }

        return View();
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
                ImageUrl = _storageService.SaveFile(formModel.Image)
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
                errors[nameof(formModel.Slug)] = "Login is required";
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
}