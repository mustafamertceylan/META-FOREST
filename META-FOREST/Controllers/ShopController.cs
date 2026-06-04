using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MetaForest.Data;
using MetaForest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MetaForest.Controllers
{
    [Authorize]
    public class ShopController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ShopController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // ========== MOCK ÜRÜN LİSTESİ ==========
        private List<ShopItem> GetMockProducts()
        {
            return new List<ShopItem>
            {
                new ShopItem { Id = 1, Name = "🐉 Altın Ejderha Teması", Description = "Ejderha bölgesi için özel tema paketi", Cost = 500, Icon = "🐉" },
                new ShopItem { Id = 2, Name = "🌳 Neon Siber Ağaç", Description = "Futuristik siber uzay ağacı", Cost = 200, Icon = "🌳" },
                new ShopItem { Id = 3, Name = "✨ Saçsız Hastalık Partikeli", Description = "Özel görsel efekt paketi", Cost = 300, Icon = "✨" },
                new ShopItem { Id = 4, Name = "🎨 Çizgi Film Sürüsü", Description = "Renkli çizgi film karakterleri", Cost = 150, Icon = "🎨" },
                new ShopItem { Id = 5, Name = "⚡ VIP Üyelik (7 gün)", Description = "7 gün boyunca ekstra coin kazanım", Cost = 1000, Icon = "⚡" }
            };
        }

        // ========== COIN BAKIYE HESAPLAMA ==========
        private async Task<int> GetUserTotalCoins(string userId)
        {
            // Kazanılan toplam coinler
            var gainedCoins = await _context.FocusSessions
                .Where(fs => fs.UserId == userId)
                .SumAsync(fs => fs.GainedCoin);

            // Harcanan toplam coinler
            var spentCoins = await _context.HarcananCoins
                .Where(hc => hc.UserId == userId)
                .SumAsync(hc => hc.SpentAmount);

            return gainedCoins - spentCoins;
        }

        // ========== MAĞAZA ANA SAYFASI ==========
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var totalCoins = await GetUserTotalCoins(userId);
            var products = GetMockProducts();

            ViewBag.TotalCoins = totalCoins;
            ViewBag.Products = products;

            return View();
        }

        // ========== ÜRÜN SATINALMA İŞLEMİ ==========
        [HttpPost]
        public async Task<IActionResult> BuyItem(string itemName, int cost)
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                var currentCoins = await GetUserTotalCoins(userId);

                // Güvenlik: Maliyeti doğrula
                if (cost <= 0)
                {
                    return Json(new { success = false, message = "Geçersiz fiyat." });
                }

                // Yeterli coin kontrolü
                if (currentCoins < cost)
                {
                    return Json(new { 
                        success = false, 
                        message = $"Yetersiz bakiye! Mevcut: {currentCoins} Coin, Gerekli: {cost} Coin" 
                    });
                }

                // Harcanan coini kaydet
                var harcananCoin = new HarcananCoin
                {
                    UserId = userId,
                    ItemName = itemName,
                    SpentAmount = cost,
                    PurchaseDate = DateTime.Now
                };

                _context.HarcananCoins.Add(harcananCoin);
                await _context.SaveChangesAsync();

                var remainingCoins = currentCoins - cost;

                return Json(new { 
                    success = true, 
                    message = $"🎉 {itemName} başarıyla satın alındı!", 
                    remainingCoins = remainingCoins 
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Hata: {ex.Message}" });
            }
        }
    }

    // ========== ÜRÜN MODELI ==========
    public class ShopItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Cost { get; set; }
        public string Icon { get; set; }
    }
}
