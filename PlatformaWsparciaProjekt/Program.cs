using PlatformaWsparciaProjekt.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;


var builder = WebApplication.CreateBuilder(args);

// Po³¹czenie z baz¹ danych
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));


// Uwierzytelnianie za pomoc¹ ciasteczek (Cookie Auth)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";      // strona logowania
        options.LogoutPath = "/Auth/Logout";    // strona wylogowania
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60); // sesja na 60 min
    });

// Dodanie kontrolerów i widoków
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

//  Kolejnoœæ ma znaczenie – najpierw autentykacja, potem autoryzacja
app.UseAuthentication();
app.UseAuthorization();

// Routing
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
