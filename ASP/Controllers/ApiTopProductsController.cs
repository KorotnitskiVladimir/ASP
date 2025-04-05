using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices.JavaScript;
using ASP.Data;
using ASP.Migrations;
using ASP.Models;
using Microsoft.AspNetCore.Mvc;
using Syncfusion.EJ2.Linq;

namespace ASP.Controllers;

[Route("api/topProducts")]
[ApiController]
public class ApiTopProductsController : Controller
{
    private readonly DataContext _dataContext;

    public ApiTopProductsController(DataContext dataContext)
    {
        _dataContext = dataContext;
    }
        
    [HttpGet]
    public RestResponse TopProducts() // название произвольное. маршрутизация по [HttpGet]
    {
        var products = _dataContext.Products
            .OrderByDescending(p => p.Stock)
            .ToList();
        return new()
        {
            Service = "Api Top Products",
            DataType = "array",
            CacheTime = 600,
            
            /*
            Data = _dataContext.Products
                .AsEnumerable()
                .Select(p => p with { ImagesCsv = "http://localhost:5089/Shop/Image/" + p.ImagesCsv.Split(',').First()})
                .OrderByDescending(p => p.Stock)
                .ToList(),
                */
                
            
            
            Data = products.Slice(0, 5)
                .Select(p => p with {ImagesCsv = "http://localhost:5089/Shop/Image/" + p.ImagesCsv
                    .Split(',').First()}),
        };
    }
}