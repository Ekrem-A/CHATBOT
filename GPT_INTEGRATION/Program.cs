using GPT_INTEGRATION.Models;
using GPT_INTEGRATION.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// OpenAI API anahtarýný appsettings.json'dan alýn
var apiKey = builder.Configuration["OpenAI:ApiKey"];

builder.Services.AddDistributedMemoryCache();  // Session için cache mekanizmasý
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);  // Oturumun zaman aþýmý süresi
    options.Cookie.HttpOnly = true;  // Cookie'lerin sadece HTTP tarafýndan eriþilebilir olmasýný saðlar
    options.Cookie.IsEssential = true;  // Cookie'nin kesinlikle gerekli olmasýný saðlar
});

builder.Services.AddDbContext<IdentityContext>(options =>
   options.UseSqlServer(builder.Configuration["ConnectionStrings:DefaultConnection"]));

//// Identity hizmetini ekleyin
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<IdentityContext>();

// OpenAIService'i Dependency Injection'a ekleyin
builder.Services.AddSingleton(new OpenAIService(apiKey));




//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//   options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//builder.Services.ConfigureApplicationCookie(options =>
//{
//    options.LoginPath = "/Account/Login";
//    options.LogoutPath = "/Account/Logout";
//    options.AccessDeniedPath = "/Account/AccessDenied";
//});

// Giriþ iþlemi için cookie kullanýmý
//builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
//    .AddCookie(options =>
//    {
//        options.LoginPath = "/Account/Login";  // Login iþlemi için yönlendirme
//        options.AccessDeniedPath = "/Account/AccessDenied";
//    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

IdentitySeedData.IdentityUser(app);


app.Run();
