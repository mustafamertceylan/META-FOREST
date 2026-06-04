using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MetaForest.Models;

namespace MetaForest.Data
{
    // ASP.NET Core Identity üyelik sistemi kullandığımız için IdentityDbContext'ten türetiyoruz
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Tablolarımızı tanımlıyoruz
        public DbSet<FocusSession> FocusSessions { get; set; }
        public DbSet<RewardAsset> RewardAssets { get; set; }
        public DbSet<PlantedAsset> PlantedAssets { get; set; }
        public DbSet<HarcananCoin> HarcananCoins { get; set; }
        public DbSet<TaskItem> TaskItems { get; set; }
    }
}