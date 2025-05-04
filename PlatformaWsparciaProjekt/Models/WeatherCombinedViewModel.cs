namespace PlatformaWsparciaProjekt.Models
{
    public class WeatherCombinedViewModel
    {
        public WeatherInfo Current { get; set; }
        public List<ForecastEntry> Forecast { get; set; }           // godzinowa
        public List<DailyForecastEntry> DailyForecast { get; set; } // dzienna
    }

    public class ForecastEntry
    {
        public DateTime Date { get; set; }
        public double Temperature { get; set; }
        public string Description { get; set; }
        public string IconUrl { get; set; }
    }

    public class DailyForecastEntry
    {
        public DateTime Date { get; set; }
        public double MinTemp { get; set; }
        public double MaxTemp { get; set; }
        public string Description { get; set; }
        public string IconUrl { get; set; }
    }
}
