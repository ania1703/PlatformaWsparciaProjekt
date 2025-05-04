namespace PlatformaWsparciaProjekt.Models
{
    public class WeatherInfo
    {
        public string City { get; set; }
        public string Description { get; set; }
        public double Temperature { get; set; }
        public int Humidity { get; set; }
        public int Pressure { get; set; }
        public double WindSpeed { get; set; }
        public string IconUrl { get; set; }
        public DateTime Sunrise { get; set; }
        public DateTime Sunset { get; set; }

    }

}

