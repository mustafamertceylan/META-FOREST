using Microsoft.EntityFrameworkCore;
using MetaForest.Data;
using Microsoft.AspNetCore.Identity;
using MetaForest.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Veritabanı Bağlantı Servisini Ekleme
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Bağlantı cümlesi 'DefaultConnection' bulunamadı.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => {
    options.Password.RequireDigit = false; // Ödev testlerinde kolay şifre verilebilsin diye esnetiyoruz
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Giriş yapılmadığında yönlendirilecek varsayılan sayfalar
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

// 2. Hava Durumu Servisi Ekleme
builder.Services.AddHttpClient<IWeatherService, WeatherService>();

// 3. MVC (Model-View-Controller) Servislerini Ekleme
builder.Services.AddControllersWithViews();

var app = builder.Build();

// 3. Tarayıcı ve İstek Ayarları (Middleware)
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // WebM videoları, CSS ve JS dosyaları için hayati önem taşır

// Veritabanı migrasyonlarını otomatik olarak uygula
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();
}

app.UseRouting();

app.UseAuthentication(); // Üyelik/Giriş sistemi için gerekli
app.UseAuthorization();

// 4. Varsayılan Sayfa Yönlendirmesi (Rota Ayarı)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

app.Run();