using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MetaForest.Data;
using MetaForest.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MetaForest.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public HomeController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string realm = "cyber")
        {
            var userId = _userManager.GetUserId(User) ?? string.Empty;

            var plantedAssets = await _context.PlantedAssets
                .Include(p => p.RewardAsset)
                .Where(p => p.UserId == userId && p.ActiveRealm == realm)
                .ToListAsync();

            ViewBag.SelectedRealm = realm;
            return View(plantedAssets);
        }

        [HttpPost]
        public async Task<IActionResult> CompleteSession(int durationMinutes, string realm, int gridX, int gridY)
        {
            var userId = _userManager.GetUserId(User) ?? string.Empty;

            int phaseLevel;
            if (durationMinutes == 0)
                phaseLevel = 1;
            else if (durationMinutes <= 30)
                phaseLevel = 2;
            else
                phaseLevel = 3;

            var cellOccupied = await _context.PlantedAssets
                .AnyAsync(p => p.UserId == userId
                            && p.ActiveRealm == realm
                            && p.GridX == gridX
                            && p.GridY == gridY);

            if (cellOccupied)
                return Json(new { success = false, message = "Bu kare artık dolu." });

            var asset = await _context.RewardAssets
                .FirstOrDefaultAsync(r => r.RealmType == realm && r.PhaseLevel == phaseLevel);

            if (asset == null)
            {
                string[] phaseNames = { "", "Fidan", "Orta Boy Ağaç", "Ulu Ağaç" };
                asset = new RewardAsset
                {
                    Name = $"{realm.ToUpper()} - {phaseNames[phaseLevel]}",
                    RealmType = realm,
                    PhaseLevel = phaseLevel,
                    WebmFileName = $"{realm}_lvl{phaseLevel}.webm"
                };
                _context.RewardAssets.Add(asset);
                await _context.SaveChangesAsync();
            }

            var session = new FocusSession
            {
                UserId = userId,
                DurationMinutes = durationMinutes,
                SessionDate = DateTime.Now,
                GainedCoin = durationMinutes * 10,
                SelectedRealm = realm
            };
            _context.FocusSessions.Add(session);

            var planted = new PlantedAsset
            {
                UserId = userId,
                RewardAssetId = asset.Id,
                GridX = gridX,
                GridY = gridY,
                ActiveRealm = realm,
                PlantedAt = DateTime.Now
            };
            _context.PlantedAssets.Add(planted);

            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                assetName = asset.Name,
                webm = asset.WebmFileName,
                coin = session.GainedCoin,
                phaseLevel = phaseLevel
            });
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminPanel()
        {
            var rewardAssets = await _context.RewardAssets.ToListAsync();
            return View(rewardAssets);
        }

        [HttpPost]
        public async Task<IActionResult> MoveAsset([FromBody] MoveAssetRequest request)
        {
            try
            {
                var userId = _userManager.GetUserId(User) ?? string.Empty;

                var asset = await _context.PlantedAssets
                    .FirstOrDefaultAsync(p => p.Id == request.AssetId);

                if (asset == null)
                    return Json(new { success = false, message = "Varlık bulunamadı." });

                if (asset.UserId != userId)
                    return Json(new { success = false, message = "Bu varlığı taşıma yetkiniz yok." });

                var targetOccupied = await _context.PlantedAssets
                    .AnyAsync(p => p.UserId == userId
                               && p.ActiveRealm == asset.ActiveRealm
                               && p.GridX == request.NewX
                               && p.GridY == request.NewY
                               && p.Id != request.AssetId);

                if (targetOccupied)
                    return Json(new { success = false, message = "Hedef kare zaten dolu." });

                asset.GridX = request.NewX;
                asset.GridY = request.NewY;
                _context.PlantedAssets.Update(asset);
                await _context.SaveChangesAsync();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Hata: {ex.Message}" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetEmptyCells(string realm)
        {
            var userId = _userManager.GetUserId(User) ?? string.Empty;

            var occupiedCells = await _context.PlantedAssets
                .Where(p => p.UserId == userId && p.ActiveRealm == realm)
                .Select(p => new { p.GridX, p.GridY })
                .ToListAsync();

            var emptyCells = from x in Enumerable.Range(0, 5)
                             from y in Enumerable.Range(0, 5)
                             where !occupiedCells.Any(o => o.GridX == x && o.GridY == y)
                             select new { x, y };

            return Json(emptyCells.ToList());
        }
    }

    public class MoveAssetRequest
    {
        public int AssetId { get; set; }
        public int NewX { get; set; }
        public int NewY { get; set; }
    }
}