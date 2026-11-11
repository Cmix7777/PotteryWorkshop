using System.Globalization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using PotteryWorkshop.Data;
using PotteryWorkshop.Logic;

// Русская культура: даты "14 ноября", цены с пробелом между разрядами
var culture = new CultureInfo("ru-RU");
CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;

// Подключаем SQLite из системы (см. Data/SqliteSetup.cs)
SqliteSetup.Init();

var builder = WebApplication.CreateBuilder(args);

// MVC: контроллеры + представления Razor
builder.Services.AddControllersWithViews();

// База данных SQLite, строка подключения в appsettings.json
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Default")));

// Вход через cookie: после логина браузер хранит зашифрованную куку с данными пользователя
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;
    });
builder.Services.AddAuthorization();

// Модули с бизнес-логикой: создаются на каждый запрос вместе с контекстом базы
builder.Services.AddScoped<BookingManager>();
builder.Services.AddScoped<OrderManager>();
builder.Services.AddScoped<EventManager>();
builder.Services.AddScoped<ReviewManager>();

var app = builder.Build();

// При запуске создаём базу (если её нет) и заполняем тестовыми данными
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
    DbSeeder.Seed(db);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Порядок важен: сначала узнаём, кто пользователь, потом проверяем права
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
