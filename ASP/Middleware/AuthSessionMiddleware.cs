using ASP.Data;

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
            //dataContext.UserAccesses
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