using Xunit;
using PlatformaWsparciaProjekt.Controllers;
using PlatformaWsparciaProjekt.Data;
using PlatformaWsparciaProjekt.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace PlatformaWsparciaProjekt.Tests.Controllers
{
    public class AuthControllerTests
    {
        private AppDbContext GetInMemoryContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new AppDbContext(options);
        }

        [Fact]
        public void Register_Get_ReturnsView()
        {
            var context = GetInMemoryContext("AuthRegisterGetDb");
            var controller = new AuthController(context);

            var result = controller.Register();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Login_Get_ReturnsView()
        {
            var context = GetInMemoryContext("AuthLoginGetDb");
            var controller = new AuthController(context);

            var result = controller.Login();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Register_Post_InvalidModel_ReturnsView()
        {
            var context = GetInMemoryContext("AuthRegisterPostInvalidDb");
            var controller = new AuthController(context);
            controller.ModelState.AddModelError("Error", "Some error");

            var result = controller.Register(new RegisterViewModel());

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.False(viewResult.ViewData.ModelState.IsValid);
        }
    }
}
