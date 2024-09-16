using GPT_INTEGRATION.Models;
using GPT_INTEGRATION.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// OpenAI API anahtar�n� appsettings.json'dan al�n
var apiKey = builder.Configuration["OpenAI:ApiKey"];

builder.Services.AddDistributedMemoryCache();  // Session i�in cache mekanizmas�
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);  // Oturumun zaman a��m� s�resi
    options.Cookie.HttpOnly = true;  // Cookie'lerin sadece HTTP taraf�ndan eri�ilebilir olmas�n� sa�lar
    options.Cookie.IsEssential = true;  // Cookie'nin kesinlikle gerekli olmas�n� sa�lar
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

// Giri� i�lemi i�in cookie kullan�m�
//builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
//    .AddCookie(options =>
//    {
//        options.LoginPath = "/Account/Login";  // Login i�lemi i�in y�nlendirme
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
