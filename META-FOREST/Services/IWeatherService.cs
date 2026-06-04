using MetaForest.Models.ViewModels;
using System.Threading.Tasks;

namespace MetaForest.Services
{
    public interface IWeatherService
    {
        /// <summary>
        /// Belirtilen şehrin hava durumu bilgisini getirir.
        /// </summary>
        Task<WeatherViewModel> GetWeatherByCityAsync(string cityName);

        /// <summary>
        /// Konfigürasyonda belirtilen şehrin hava durumu bilgisini getirir.
        /// </summary>
        Task<WeatherViewModel> GetWeatherAsync();

        /// <summary>
        /// Hava durumuna göre ödül çarpanını döner.
        /// Rain: 1.5x, Snow: 1.2x, Thunderstorm: 1.3x, Clear: 1.0x, Cloudy: 1.0x
        /// </summary>
        Task<double> GetMoneyMultiplierAsync();
    }
}
