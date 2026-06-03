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
            var userId = _userManager.GetUserId(User);

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
            var userId = _userManager.GetUserId(User);

            int phaseLevel = 1;
            if (durationMinutes >= 30 && durationMinutes < 60) phaseLevel = 2;
            if (durationMinutes >= 60) phaseLevel = 3;

            var asset = await _context.RewardAssets
                .FirstOrDefaultAsync(r => r.RealmType == realm && r.PhaseLevel == phaseLevel);

            if (asset == null)
            {
                asset = new RewardAsset
                {
                    Name = $"{realm.ToUpper()} - Faz {phaseLevel}",
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
                GainedCoin = durationMinutes * 10
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

            return Json(new { success = true, assetName = asset.Name, webm = asset.WebmFileName, coin = session.GainedCoin });
        }

        [Authorize(Roles = "Admin")]
        public IActionResult AdminPanel()
        {
            return View();
        }
    }
}