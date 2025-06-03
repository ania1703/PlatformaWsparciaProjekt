using Xunit;
using PlatformaWsparciaProjekt.Controllers;
using PlatformaWsparciaProjekt.Data;
using PlatformaWsparciaProjekt.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;

namespace PlatformaWsparciaProjekt.Tests.Controllers
{
    public class RatingControllerTests
    {
        private RatingController GetControllerWithContext(AppDbContext context, int userId = 1, string role = "Senior")
        {
            var controller = new RatingController(context);
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Role, role)
            }, "mock"));

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            return controller;
        }

        private AppDbContext GetInMemoryContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new AppDbContext(options);
        }

        [Fact]
        public void Index_Returns_ViewResult()
        {
            var context = GetInMemoryContext("RatingIndexDb");
            var controller = GetControllerWithContext(context);

            var result = controller.Index();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Create_Get_Returns_View()
        {
            var context = GetInMemoryContext("HelpRequestCreateDb");
            var controller = GetControllerWithContext(context);

            var result = controller.Create();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void VolunteerDetails_Returns_ViewResult_When_NoRatings()
        {
            var context = GetInMemoryContext("VolunteerDetailsDb");

            // Dodajemy wolontariusza do bazy
            context.Volunteers.Add(new Volunteer
            {
                Id = 1,
                FirstName = "Anna",
                LastName = "Kowalska",
                Email = "anna@example.com",
                Password = "password",
                Phone = "987654321"
            });
            context.SaveChanges();

            var controller = GetControllerWithContext(context);

            var result = controller.VolunteerDetails(1); // istniejący wolontariusz

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<Rating>>(viewResult.Model);
            Assert.Empty(model);  // bo nie dodaliśmy żadnych ocen
        }


        [Fact]
        public void Create_Get_ReturnsView_WithVolunteers()
        {
            var context = GetInMemoryContext("RatingCreateDb");
            context.Volunteers.Add(new Volunteer
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                Password = "password",
                Phone = "123456789"
            });
            context.SaveChanges();

            var controller = GetControllerWithContext(context, role: "Senior");

            var result = controller.Create();

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.NotNull(controller.ViewBag.Volunteers);
        }
    }
}
