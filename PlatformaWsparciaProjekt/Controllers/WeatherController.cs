using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using PlatformaWsparciaProjekt.Models;

namespace PlatformaWsparciaProjekt.Controllers
{
    public class WeatherController : Controller
    {
        private readonly IConfiguration _configuration;

        public WeatherController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IActionResult> Index(string city = "Warsaw")
        {
            string apiKey = _configuration["OpenWeatherMap:ApiKey"];
            string url = $"https://api.openweathermap.org/data/2.5/weather={city}&appid={apiKey}&units=metric";


            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetStringAsync(url);
                var json = JObject.Parse(response);

                var model = new WeatherInfo
                {
                    City = city,
                    Temperature = json["main"]["temp"].Value<double>(),
                    Description = json["weather"][0]["description"].ToString(),
                    IconUrl = $"http://openweathermap.org/img/wn/{json["weather"][0]["icon"]}@2x.png"
                };

                return View(model);
            }
        }
    }
}

