using ASP.Data;
using ASP.Models;
using Microsoft.AspNetCore.Mvc;

namespace ASP.Controllers;

[Route("api/product")]
[ApiController]
public class ApiProductController : ControllerBase
{
    private readonly DataAccessor _dataAccessor;

    public ApiProductController(DataAccessor dataAccessor)
    {
        _dataAccessor = dataAccessor;
    }

    [HttpGet("{id}")]
    public RestResponse Product(string id)
    {
        return new()
        {
            Service = "Api Products",
            DataType = "object",
            CacheTime = 600,
            Data = _dataAccessor.GetProduct(id)
        };
    }
}