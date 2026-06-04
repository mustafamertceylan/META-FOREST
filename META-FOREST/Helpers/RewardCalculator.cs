using MetaForest.Services;
using System.Threading.Tasks;

namespace MetaForest.Helpers
{
    /// <summary>
    /// Hava durumuna göre ödül hesaplamasını yardımcı metotlar ile sağlar.
    /// HomeController'da kullanılmak üzere tasarlandı.
    /// </summary>
    public class RewardCalculator
    {
        private readonly IWeatherService _weatherService;

        public RewardCalculator(IWeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        /// <summary>
        /// Hava durumuna göre para ödülünü hesapla.
        /// Örn: 100 para + 1.5x çarpan = 150 para
        /// </summary>
        public async Task<decimal> CalculateMoneyRewardAsync(decimal baseMoney)
        {
            if (baseMoney <= 0)
                return 0;

            var multiplier = await _weatherService.GetMoneyMultiplierAsync();
            return baseMoney * (decimal)multiplier;
        }

        /// <summary>
        /// Hava durumunun bonus açıklamasını döner.
        /// Örn: "☔ Yağmur Bonusu (+50%)" 
        /// </summary>
        public async Task<string> GetWeatherBonusDescriptionAsync()
        {
            var multiplier = await _weatherService.GetMoneyMultiplierAsync();
            var bonusPercentage = (multiplier - 1.0) * 100;

            return multiplier switch
            {
                1.5 => "☔ Yağmur Bonusu (+50%)",
                1.3 => "⛈️ Fırtına Bonusu (+30%)",
                1.2 => "❄️ Kar Bonusu (+20%)",
                1.0 => "🌤️ Normal Ödül (Bonus Yok)",
                _ => $"🎯 Hava Bonusu (+{bonusPercentage:F0}%)"
            };
        }

        /// <summary>
        /// Bonus tutarını hesapla.
        /// Örn: 100 para -> 150 para = 50 bonus
        /// </summary>
        public async Task<decimal> GetBonusAmountAsync(decimal baseMoney)
        {
            if (baseMoney <= 0)
                return 0;

            var finalReward = await CalculateMoneyRewardAsync(baseMoney);
            return finalReward - baseMoney;
        }
    }
}
