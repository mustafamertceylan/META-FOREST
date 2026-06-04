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
    }
}
