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
    try
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        dbContext.Database.Migrate();

        // TaskItems tablosunu oluştur (varsa yok sayar)
        dbContext.Database.ExecuteSqlRaw(@"
            IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'TaskItems')
            BEGIN
                CREATE TABLE TaskItems (
                    Id INT PRIMARY KEY IDENTITY(1,1),
                    UserId NVARCHAR(MAX) NOT NULL,
                    Title NVARCHAR(255) NOT NULL,
                    Description NVARCHAR(1000) NULL,
                    IsCompleted BIT NOT NULL DEFAULT 0,
                    CreatedAt DATETIME2 NOT NULL,
                    UpdatedAt DATETIME2 NOT NULL,
                    Priority NVARCHAR(20) NOT NULL DEFAULT 'Medium'
                )
            END
        ");
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Veritabanı migrasyon sırasında hata oluştu");
    }
}

app.UseRouting();

app.UseAuthentication(); // Üyelik/Giriş sistemi için gerekli
app.UseAuthorization();

// 4. Varsayılan Sayfa Yönlendirmesi (Rota Ayarı)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Landing}/{id?}");

app.Run();