using MetaForest.Models.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace MetaForest.Services
{
    public class WeatherService : IWeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<WeatherService> _logger;
        private readonly Dictionary<string, string> _weatherEmojis = new()
        {
            { "clear", "☀️" },
            { "clouds", "☁️" },
            { "rain", "🌧️" },
            { "drizzle", "🌦️" },
            { "thunderstorm", "⛈️" },
            { "snow", "❄️" },
            { "mist", "🌫️" },
            { "smoke", "💨" },
            { "haze", "🌫️" },
            { "dust", "🌪️" },
            { "fog", "🌫️" },
            { "sand", "🌪️" },
            { "ash", "💨" },
            { "squall", "💨" },
            { "tornado", "🌪️" }
        };

        public WeatherService(HttpClient httpClient, IConfiguration configuration, ILogger<WeatherService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<WeatherViewModel> GetWeatherByCityAsync(string cityName)
        {
            try
            {
                var apiKey = _configuration["WeatherSettings:ApiKey"];
                var baseUrl = _configuration["WeatherSettings:BaseUrl"];
                var units = _configuration["WeatherSettings:Units"] ?? "metric";

                if (string.IsNullOrEmpty(apiKey) || apiKey == "YOUR_OPENWEATHERMAP_API_KEY")
                {
                    _logger.LogWarning("OpenWeatherMap API Key not configured properly.");
                    return new WeatherViewModel
                    {
                        IsError = true,
                        ErrorMessage = "Hava durumu API anahtarı yapılandırılmamış.",
                        CityName = cityName,
                        FormattedWeather = "API Key hatası"
                    };
                }

                var url = $"{baseUrl}?q={cityName}&appid={apiKey}&units={units}&lang=tr";

                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning($"Weather API returned status code: {response.StatusCode}");
                    return new WeatherViewModel
                    {
                        IsError = true,
                        ErrorMessage = "Hava durumu bilgisi alınamadı.",
                        CityName = cityName,
                        FormattedWeather = "⚠️ Bilgi alınamadı"
                    };
                }

                var content = await response.Content.ReadAsStringAsync();
                var jsonDoc = JsonDocument.Parse(content);
                var root = jsonDoc.RootElement;

                var weatherData = new WeatherViewModel
                {
                    CityName = root.GetProperty("name").GetString(),
                    Temperature = Math.Round(root.GetProperty("main").GetProperty("temp").GetDouble(), 0),
                    FeelsLike = Math.Round(root.GetProperty("main").GetProperty("feels_like").GetDouble(), 0),
                    Humidity = root.GetProperty("main").GetProperty("humidity").GetInt32(),
                    WindSpeed = Math.Round(root.GetProperty("wind").GetProperty("speed").GetDouble(), 1),
                    IsError = false
                };

                if (root.TryGetProperty("weather", out var weatherArray) && weatherArray.GetArrayLength() > 0)
                {
                    var weather = weatherArray[0];
                    weatherData.WeatherDescription = weather.GetProperty("main").GetString();
                    var description = weather.GetProperty("description").GetString();

                    // Get emoji for weather
                    var weatherMain = weatherData.WeatherDescription.ToLower();
                    weatherData.WeatherIcon = _weatherEmojis.ContainsKey(weatherMain) 
                        ? _weatherEmojis[weatherMain] 
                        : "🌤️";
                }

                // Format the weather string for display
                weatherData.FormattedWeather = $"{weatherData.WeatherIcon} {weatherData.CityName} {weatherData.Temperature}°C";

                return weatherData;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request error while fetching weather data.");
                return new WeatherViewModel
                {
                    IsError = true,
                    ErrorMessage = $"Network error: {ex.Message}",
                    CityName = cityName,
                    FormattedWeather = "🔌 Bağlantı hatası"
                };
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "JSON parsing error while processing weather response.");
                return new WeatherViewModel
                {
                    IsError = true,
                    ErrorMessage = $"Data parsing error: {ex.Message}",
                    CityName = cityName,
                    FormattedWeather = "⚠️ Veri hatası"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while fetching weather data.");
                return new WeatherViewModel
                {
                    IsError = true,
                    ErrorMessage = $"Unexpected error: {ex.Message}",
                    CityName = cityName,
                    FormattedWeather = "❓ Bilinmeyen hata"
                };
            }
        }

        public async Task<WeatherViewModel> GetWeatherAsync()
        {
            var cityName = _configuration["WeatherSettings:CityName"] ?? "Istanbul";
            return await GetWeatherByCityAsync(cityName);
        }

        public async Task<double> GetMoneyMultiplierAsync()
        {
            try
            {
                var weather = await GetWeatherAsync();

                if (weather == null || weather.IsError)
                {
                    // Hava durumu alınamadıysa varsayılan 1.0x çarpanı döner
                    return 1.0;
                }

                // Hava durumuna göre çarpanı belirle
                var weatherMain = weather.WeatherDescription?.ToLower() ?? "";

                return weatherMain switch
                {
                    // Yağmur en yüksek çarpanı (1.5x)
                    "rain" => 1.5,
                    "rainy" => 1.5,
                    "drizzle" => 1.3,

                    // Kar çarpanı (1.2x)
                    "snow" => 1.2,
                    "snowy" => 1.2,

                    // Fırtına çarpanı (1.3x)
                    "thunderstorm" => 1.3,
                    "storm" => 1.3,

                    // Bulutlu ve berrak - normal çarpan (1.0x)
                    "clouds" => 1.0,
                    "cloudy" => 1.0,
                    "clear" => 1.0,
                    "sunny" => 1.0,
                    "mist" => 1.0,
                    "fog" => 1.0,

                    // Varsayılan
                    _ => 1.0
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Para çarpanı hesaplanırken hata oluştu.");
                return 1.0; // Hata durumunda varsayılan çarpan
            }
        }
    }
}
