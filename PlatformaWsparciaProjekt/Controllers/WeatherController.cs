using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using PlatformaWsparciaProjekt.Models;

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
        var currentUrl = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={apiKey}&units=metric&lang=pl";
        var forecastUrl = $"https://api.openweathermap.org/data/2.5/forecast?q={city}&appid={apiKey}&units=metric&lang=pl";

        using (HttpClient client = new HttpClient())
        {
            try
            {
                var currentResponse = await client.GetAsync(currentUrl);
                var forecastResponse = await client.GetAsync(forecastUrl);

                if (!currentResponse.IsSuccessStatusCode || !forecastResponse.IsSuccessStatusCode)
                {
                    ViewBag.Error = "Błąd podczas pobierania danych z API.";
                    return View();
                }

                var currentContent = await currentResponse.Content.ReadAsStringAsync();
                var forecastContent = await forecastResponse.Content.ReadAsStringAsync();

                var currentJson = JObject.Parse(currentContent);
                var forecastJson = JObject.Parse(forecastContent);

                var current = new WeatherInfo
                {
                    City = city,
                    Temperature = currentJson["main"]["temp"].Value<double>(),
                    Description = currentJson["weather"][0]["description"].ToString(),
                    IconUrl = $"http://openweathermap.org/img/wn/{currentJson["weather"][0]["icon"]}@2x.png",
                    Humidity = currentJson["main"]["humidity"].Value<int>(),
                    Pressure = currentJson["main"]["pressure"].Value<int>(),
                    WindSpeed = currentJson["wind"]["speed"].Value<double>(),
                    Sunrise = DateTimeOffset.FromUnixTimeSeconds(currentJson["sys"]["sunrise"].Value<long>()).LocalDateTime,
                    Sunset = DateTimeOffset.FromUnixTimeSeconds(currentJson["sys"]["sunset"].Value<long>()).LocalDateTime
                };

                var forecastList = new List<ForecastEntry>();
                foreach (var item in forecastJson["list"])
                {
                    forecastList.Add(new ForecastEntry
                    {
                        Date = DateTime.Parse(item["dt_txt"].ToString()),
                        Temperature = item["main"]["temp"].Value<double>(),
                        Description = item["weather"][0]["description"].ToString(),
                        IconUrl = $"http://openweathermap.org/img/wn/{item["weather"][0]["icon"]}@2x.png"
                    });
                }

                // Grupowanie po dniach dla prognozy dziennej
                var dailyForecast = forecastList
                    .GroupBy(f => f.Date.Date)
                    .Select(g => new DailyForecastEntry
                    {
                        Date = g.Key,
                        MinTemp = g.Min(x => x.Temperature),
                        MaxTemp = g.Max(x => x.Temperature),
                        Description = g.First().Description,
                        IconUrl = g.First().IconUrl
                    })
                    .Take(5)
                    .ToList();

                var model = new WeatherCombinedViewModel
                {
                    Current = current,
                    Forecast = forecastList.Take(9).ToList(), // najbliższe 27h co 3h
                    DailyForecast = dailyForecast
                };

                ViewBag.SelectedCity = city;

                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Nie udało się pobrać danych pogodowych.";
                ViewBag.Exception = ex.Message;
                return View();
            }
        }
    }

}
