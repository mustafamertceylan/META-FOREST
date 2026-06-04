/**
 * WEATHER-BASED REWARD SYSTEM - HOMECONTROLLER KULLANIM ÖRNEĞI
 * 
 * Bu dosya, HomeController'daki CompleteSession metodunda
 * hava durumuna göre para çarpanını nasıl uygulayacağınızı gösterir.
 * 
 * ⚠️ UYARI: HomeController'a dokunmamak kuralına uyduğumuzdan,
 * bu dosya sadece REFERANS olarak hazırlanmıştır!
 */

// ============================================
// STEP 1: Controller'a dependency inject et
// ============================================

public class HomeController : Controller
{
	private readonly RewardCalculator _rewardCalculator;

	public HomeController(RewardCalculator rewardCalculator)
	{
		_rewardCalculator = rewardCalculator;
	}

	// ============================================
	// STEP 2: CompleteSession metodunda kullan
	// ============================================

	[HttpPost]
	public async Task<IActionResult> CompleteSession(int durationMinutes, string realm, int gridX, int gridY)
	{
		try
		{
			// 1. Taban ödülünü hesapla (örn: dakika * 10 = para)
			decimal baseMoney = durationMinutes * 10;

			// 2. Hava durumuna göre para çarpanını al ve son ödülü hesapla
			decimal finalMoney = await _rewardCalculator.CalculateMoneyRewardAsync(baseMoney);
			decimal bonusAmount = await _rewardCalculator.GetBonusAmountAsync(baseMoney);
			string bonusDescription = await _rewardCalculator.GetWeatherBonusDescriptionAsync();

			// 3. Veritabanına kaydet
			var focusSession = new FocusSession
			{
				UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
				DurationMinutes = durationMinutes,
				MoneyEarned = finalMoney,  // Çarpanlı para
				WeatherBonus = bonusAmount,  // Bonus tutarı
				RealmType = realm,
				GridX = gridX,
				GridY = gridY,
				CreatedAt = DateTime.Now
			};

			_dbContext.FocusSessions.Add(focusSession);
			await _dbContext.SaveChangesAsync();

			// 4. Kullanıcıya başarı mesajı dön
			return Json(new
			{
				success = true,
				assetName = GetRandomAsset(realm),
				money = finalMoney,
				baseReward = baseMoney,
				bonus = bonusAmount,
				bonusDescription = bonusDescription,
				message = $"🎉 {bonusDescription} ile {finalMoney} para kazandınız!"
			});
		}
		catch (Exception ex)
		{
			return Json(new { success = false, error = ex.Message });
		}
	}
}

// ============================================
// ÖRNEK: RewardCalculator Kullanımı
// ============================================

/*
 * Senaryo 1: Yağmur Havası (1.5x çarpan)
 * - Taban ödül: 100 para
 * - Son ödül: 100 * 1.5 = 150 para
 * - Bonus: 50 para
 * - Açıklama: "☔ Yağmur Bonusu (+50%)"
 */

decimal baseMoney = 100;
decimal finalMoney = await _rewardCalculator.CalculateMoneyRewardAsync(baseMoney);  // 150
decimal bonus = await _rewardCalculator.GetBonusAmountAsync(baseMoney);  // 50
string description = await _rewardCalculator.GetWeatherBonusDescriptionAsync();  // "☔ Yağmur Bonusu (+50%)"

/*
 * Senaryo 2: Kar Havası (1.2x çarpan)
 * - Taban ödül: 100 para
 * - Son ödül: 100 * 1.2 = 120 para
 * - Bonus: 20 para
 * - Açıklama: "❄️ Kar Bonusu (+20%)"
 */

/*
 * Senaryo 3: Berrak Hava (1.0x çarpan - Bonus Yok)
 * - Taban ödül: 100 para
 * - Son ödül: 100 * 1.0 = 100 para
 * - Bonus: 0 para
 * - Açıklama: "🌤️ Normal Ödül (Bonus Yok)"
 */

// ============================================
// GETMONEYYMULTIPLIER REFERANSI
// ============================================

/*
 * GetMoneyMultiplierAsync() çarpan tablosu:
 * 
 * Rain (Yağmur)           → 1.5x (En Yüksek)
 * Thunderstorm (Fırtına)  → 1.3x
 * Snow (Kar)              → 1.2x
 * Drizzle (Çiseleme)      → 1.3x
 * Clear (Berrak)          → 1.0x
 * Cloudy (Bulutlu)        → 1.0x
 * Mist/Fog (Sis)          → 1.0x
 * Hata Durumu             → 1.0x (Fallback)
 */

// ============================================
// VERİTABANI ŞEMASI (FocusSession Örneği)
// ============================================

/*
 * public class FocusSession
 * {
 *     public int Id { get; set; }
 *     public string UserId { get; set; }
 *     public int DurationMinutes { get; set; }
 *     public decimal MoneyEarned { get; set; }  // ← Çarpanlı para
 *     public decimal WeatherBonus { get; set; }  // ← Bonus tutarı
 *     public string RealmType { get; set; }
 *     public int GridX { get; set; }
 *     public int GridY { get; set; }
 *     public DateTime CreatedAt { get; set; }
 * }
 */
