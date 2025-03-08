using System.Security.Claims;
using ASP.Data;
using ASP.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ASP.Middleware;

public class AuthSessionMiddleware
{
    private readonly RequestDelegate _next;

    public AuthSessionMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    
    // !! Инжекция в Middleware осуществляется через метод, не через конструктор (конструктор "занят")

    public async Task InvokeAsync(HttpContext context, DataContext dataContext)
    {
        if (context.Request.Query.ContainsKey("logout"))
        {
            context.Session.Remove("userAccessId");
            context.Response.Redirect(context.Request.Path);
            return;
        }
        if (context.Session.Keys.Contains("userAccessId"))
        {
            // пользователь аутентифицирован
            context.Items.Add("auth", "OK");
            // В сессии только ID, находим все данные про пользователя

            if (dataContext.UserAccesses
                    .Include(ua => ua.UserData) // команды на заполнение навигационных свойств
                    .Include(ua => ua.UserRole)
                    .FirstOrDefault(ua =>
                        ua.Id.ToString() == context.Session.GetString("userAccessId"))
                is UserAccess userAccess)
            {
                //dataContext.UserAccesses
                // Claims - данные, которые передаются в контекст по единой схеме: пары ключ - значение, предназначенные
                // для задач авторизации
                context.User = new ClaimsPrincipal(
                    new ClaimsIdentity(
                        new Claim[]
                        {
                            new Claim(ClaimTypes.Sid, userAccess.Id.ToString()),
                            new Claim(ClaimTypes.Name, userAccess.UserData.Name),
                            new Claim(ClaimTypes.Email, userAccess.UserData.Email),
                            new Claim(ClaimTypes.MobilePhone, userAccess.UserData.Phone),
                            new Claim(ClaimTypes.Role, userAccess.UserRole.Id),
                            new Claim("CanCreate", userAccess.UserRole.CanCreate.ToString()),
                            new Claim("CanRead", userAccess.UserRole.CanRead.ToString()),
                            new Claim("CanUpdate", userAccess.UserRole.CanUpdate.ToString()),
                            new Claim("CanDelete", userAccess.UserRole.CanDelete.ToString())
                        },
                        nameof(AuthSessionMiddleware)));
            }
        }
        // Call the next delegate/middleware in the pipeline.
        await _next(context);
    }
}

public static class AuthSessionMiddlewareExtensions
{
    public static IApplicationBuilder UseAuthSession(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<AuthSessionMiddleware>();     
    }
}