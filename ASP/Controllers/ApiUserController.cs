using System.ComponentModel;
using System.Text;
using System.Text.Json;
using ASP.Data;
using ASP.Data.Entities;
using ASP.Middleware;
using ASP.Models;
using ASP.Models.User;
using Microsoft.AspNetCore.Authentication;
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

    [HttpGet("jti")]
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
    
    [HttpGet]
    public RestResponse AuthenticateJwt()
    {
        var res = new RestResponse()
        {
            Service = "Api User Authentication",
            DataType = "object",
            CacheTime = 600,
        };
        try
        {
            string header = Base64UrlTextEncoder.Encode(
                Encoding.UTF8.GetBytes("{  \"alg\": \"HS256\",  \"typ\": \"JWT\"}"));
            string payload = Base64UrlTextEncoder.Encode(
                Encoding.UTF8.GetBytes(
                    JsonSerializer.Serialize(_dataAccessor.Authenticate(Request))));
            string data = header + "." + payload;
            string signature = Base64UrlTextEncoder.Encode(System.Security.Cryptography.HMACSHA256.HashData(
                Encoding.UTF8.GetBytes("secret"),
                Encoding.UTF8.GetBytes(data)));
            res.Data = data + "." + signature;
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

    [HttpPost]
    public RestResponse SignUp(UserApiSignUpFormModel model)
    {
        var res = new RestResponse()
        {
            Service = "API user registration",
            DataType = "object",
            CacheTime = 0,
            Data = model
        };
        return res;
    }

    [HttpGet("profile")]
    public RestResponse Profile()
    {
        var res = new RestResponse()
        {
            Service = "Api User Profile",
            DataType = "object",
            CacheTime = 600
        };
        if (HttpContext.User.Identity?.IsAuthenticated ?? false)
        {
           res.Data = (HttpContext.Items["AccessToken"] as AccessToken)?.User;
        }
        else
        {
            res.Status = new()
            {
                IsOk = false,
                Code = 401,
                Phrase = HttpContext.Items[nameof(AuthTokenMiddleware)]?.ToString() ?? ""
            };
        }
        return res;
    }
}