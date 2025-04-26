using ASP.Data;
using ASP.Middleware;
using ASP.Services.KDF;
using ASP.Services.PasswordGenerator;
using ASP.Services.Storage;
using ASP.Services.Timestamp;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();

builder.Services.AddSingleton<IKDFService, PBKDF1Service>();

// Регистрируем сервисы
// builder.Services.AddSingleton<ITimestampService, SystemTimestampService>();
// пример замены сервиса SystemTimestampService на UnixTimestampService
builder.Services.AddSingleton<ITimestampService, UnixTimestampService>();
// builder.Services.AddTransient<ITimestampService, UnixTimestampService>(); каждый раз создаются разные объекты
builder.Services.AddSingleton<IPasswordGeneratorService, PasswordGenerator>();

builder.Services.AddSingleton<IStorageService, FileStorageService>();

// Включение сессии - длительного хранилища, что позволяет сохранять данные между запросами
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Регистрация контекста данных - аналогична сервисам
builder.Services.AddDbContext<DataContext>(         // Метод регистрации - AddDbContext 
    options => options                   // options - то, что передается в конструктор 
        .UseSqlServer(builder                       // DataContext(DbContextOptions)
            .Configuration                          // UseSqlServer - конфигурация для MS SQL Server 
            .GetConnectionString("LocalMs"))); // builder.Configuration - доступ к файлам конфигурации (appsettings.json)

builder.Services.AddScoped<DataAccessor>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        policy  =>
        {
            policy.AllowAnyOrigin().AllowAnyHeader();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("CorsPolicy");
app.UseAuthorization();

app.UseSession(); // включение сессии

app.UseAuthSession();

app.UseAuthToken();

app.MapStaticAssets();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();

