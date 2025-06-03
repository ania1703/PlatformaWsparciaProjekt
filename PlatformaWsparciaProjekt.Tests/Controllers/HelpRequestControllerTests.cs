using Xunit;
using PlatformaWsparciaProjekt.Controllers;
using PlatformaWsparciaProjekt.Data;
using PlatformaWsparciaProjekt.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace PlatformaWsparciaProjekt.Tests.Controllers
{
    public class HelpRequestControllerTests
    {
        private HelpRequestController GetControllerWithContext(AppDbContext context, int userId = 1, string role = "Senior")
        {
            var controller = new HelpRequestController(context);
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
            var context = GetInMemoryContext("HelpRequestIndexDb");
            var controller = GetControllerWithContext(context);

            var result = controller.Index(null, null);

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Details_Returns_NotFound_When_Id_NotExist()
        {
            var context = GetInMemoryContext("HelpRequestDetailsDb");
            var controller = GetControllerWithContext(context);

            var result = controller.Details(999);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Edit_Returns_NotFound_When_Id_NotExist()
        {
            var context = GetInMemoryContext("HelpRequestEditDb");
            var controller = GetControllerWithContext(context, role: "Senior");

            var result = controller.Edit(999);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Delete_Returns_NotFound_When_Id_NotExist()
        {
            var context = GetInMemoryContext("HelpRequestDeleteDb");
            var controller = GetControllerWithContext(context, role: "Senior");

            var result = controller.Delete(999);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void AddVolunteer_Assigns_Volunteer()
        {
            var context = GetInMemoryContext("HelpRequestAddVolunteerDb");

            // Dodaj seniora z wymaganymi polami
            context.Seniors.Add(new Senior
            {
                Id = 1,
                FirstName = "Test",
                LastName = "Senior",
                Email = "senior@example.com",
                Password = "password",
                Phone = "123456789",
                Age = 70,
                Address = "Test Address",  // wymagane
                Bio = "Test Bio"           // wymagane
            });

            // Dodaj wolontariusza z wymaganymi polami
            context.Volunteers.Add(new Volunteer
            {
                Id = 1,
                FirstName = "Test",
                LastName = "Volunteer",
                Email = "volunteer@example.com",
                Password = "password",
                Phone = "987654321"
            });

            // Dodaj zgłoszenie z wymaganymi polami
            context.HelpRequests.Add(new HelpRequest
            {
                Id = 1,
                SeniorId = 1,
                Category = "Zakupy",
                Description = "Potrzebuję zakupów",
                Priority = "Wysoki",
                Title = "Zakupy spożywcze"
            });

            context.SaveChanges();

            var controller = GetControllerWithContext(context, userId: 1, role: "Volunteer");

            // Act
            var result = controller.AddVolunteer(1);

            // Assert
            var updatedRequest = context.HelpRequests.First(hr => hr.Id == 1);
            Assert.Equal(1, updatedRequest.VolunteerId);
        }


        [Fact]
        public void Details_ReturnsView_WhenRequestExists()
        {
            var context = GetInMemoryContext("HelpRequestDetailsExistsDb");

            context.HelpRequests.Add(new HelpRequest
            {
                Id = 1,
                Title = "Test Request",
                Category = "Zakupy",
                Description = "Potrzebuję pomocy",
                Priority = "High",
                SeniorId = 1
            });
            context.SaveChanges();

            var controller = GetControllerWithContext(context);

            var result = controller.Details(1);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<HelpRequest>(viewResult.Model);
            Assert.Equal(1, model.Id);
        }


        [Fact]
        public void Index_SortsByDateAscending()
        {
            var context = GetInMemoryContext("HelpRequestSortDb");
            context.HelpRequests.Add(new HelpRequest
            {
                Id = 1,
                Title = "First",
                Category = "Zakupy",
                Description = "Potrzebuję zakupów",
                Priority = "Wysoki",
                CreatedAt = DateTime.Now.AddDays(-1)
            });
            context.HelpRequests.Add(new HelpRequest
            {
                Id = 2,
                Title = "Second",
                Category = "Zakupy",
                Description = "Potrzebuję zakupów",
                Priority = "Wysoki",
                CreatedAt = DateTime.Now
            });
            context.SaveChanges();

            var controller = GetControllerWithContext(context);

            var result = controller.Index("", null) as ViewResult;
            var model = Assert.IsAssignableFrom<List<HelpRequest>>(result.Model);
            Assert.Equal(1, model.First().Id);
        }



        [Fact]
        public void Create_Get_ReturnsView_ForSenior()
        {
            var context = GetInMemoryContext("HelpRequestCreateDb");
            var controller = GetControllerWithContext(context, role: "Senior");

            var result = controller.Create();

            Assert.IsType<ViewResult>(result);
        }
    }
}

