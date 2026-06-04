namespace MetaForest.Models.ViewModels
{
    public class WeatherViewModel
    {
        public string CityName { get; set; }
        public string WeatherDescription { get; set; }
        public string WeatherIcon { get; set; }
        public double Temperature { get; set; }
        public double FeelsLike { get; set; }
        public int Humidity { get; set; }
        public double WindSpeed { get; set; }
        public string FormattedWeather { get; set; }
        public bool IsError { get; set; }
        public string ErrorMessage { get; set; }
    }
}
