using System.ComponentModel;
using ASP.Data;
using ASP.Models;
using Microsoft.AspNetCore.Mvc;

namespace ASP.Controllers;

[Route("api/user")]
[ApiController]
public class ApiUserController : ControllerBase
{
    private readonly DataAccessor _dataAccessor;

    public ApiUserController(DataAccessor dataAccessor)
    {
        _dataAccessor = dataAccessor;
    }

    [HttpGet]
    public RestResponse Authenticate()
    {
        var res = new RestResponse()
        {
            Service = "Api User Authentication",
            DataType = "object",
            CacheTime = 600,
        };
        try
        {
            res.Data = _dataAccessor.Authenticate(Request);
        }
        catch (Win32Exception e)
        {
            res.Status = new()
            {
                IsOk = false,
                Code = e.ErrorCode,
                Phrase = e.Message
            };
            res.Data = null;
        }

        return res;
    }
}