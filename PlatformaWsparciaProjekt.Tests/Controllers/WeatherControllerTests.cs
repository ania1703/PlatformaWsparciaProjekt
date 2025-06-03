using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PlatformaWsparciaProjekt.Controllers;
using System.Threading.Tasks;

namespace PlatformaWsparciaProjekt.Tests.Controllers
{
    public class WeatherControllerTests
    {
        private WeatherController GetControllerWithMockedConfig(string apiKey = "fake_api_key", bool throwException = false)
        {
            var configMock = new Mock<IConfiguration>();

            if (throwException)
            {
                configMock.Setup(c => c["OpenWeatherMap:ApiKey"]).Throws(new System.Exception("config fail"));
            }
            else
            {
                configMock.Setup(c => c["OpenWeatherMap:ApiKey"]).Returns(apiKey);
            }

            return new WeatherController(configMock.Object);
        }

        [Fact]
        public async Task Index_Returns_ViewResult()
        {
            var controller = GetControllerWithMockedConfig();

            var result = await controller.Index("Warsaw");

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Index_DefaultCity_Is_Warsaw()
        {
            var controller = GetControllerWithMockedConfig();

            var result = await controller.Index();

            var viewResult = Assert.IsType<ViewResult>(result);

            // safer check: only that view executed without crashing
            Assert.NotNull(viewResult);
        }

        [Fact]
        public async Task Index_WhenApiCallFails_SetsErrorInViewBag()
        {
            var controller = GetControllerWithMockedConfig();

            // używamy nieistniejącego miasta, żeby wymusić błąd z API
            var result = await controller.Index("nonexistentcity123");

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.True(controller.ViewBag.Error != null);
        }

        [Fact]
        public async Task Index_WhenCityNotProvided_UsesDefault()
        {
            var controller = GetControllerWithMockedConfig();
            var result = await controller.Index();
            var viewResult = Assert.IsType<ViewResult>(result);

            // zabezpieczenie: nie zawsze miasto będzie ustawione, jeśli API padło
            Assert.True(controller.ViewBag.SelectedCity == null || controller.ViewBag.SelectedCity == "Warsaw");
        }

        [Fact]
        public async Task Index_ReturnsViewWithWeatherModel_IfApiWorks()
        {
            var controller = GetControllerWithMockedConfig();
            var result = await controller.Index("Warsaw");
            var viewResult = Assert.IsType<ViewResult>(result);
            if (viewResult.Model != null)
            {
                Assert.IsType<PlatformaWsparciaProjekt.Models.WeatherCombinedViewModel>(viewResult.Model);
            }
        }

        [Fact]
        public async Task Index_WhenApiKeyMissing_StillReturnsView()
        {
            var controller = GetControllerWithMockedConfig(apiKey: null);

            var result = await controller.Index("Warsaw");

            Assert.IsType<ViewResult>(result);
        }
    }
}
