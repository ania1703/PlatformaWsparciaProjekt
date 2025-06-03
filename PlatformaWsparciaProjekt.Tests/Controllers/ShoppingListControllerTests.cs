using Xunit;
using PlatformaWsparciaProjekt.Controllers;
using PlatformaWsparciaProjekt.Data;
using PlatformaWsparciaProjekt.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace PlatformaWsparciaProjekt.Tests.Controllers
{
    public class ShoppingListControllerTests
    {
        private ShoppingListController GetControllerWithContext(AppDbContext context, int userId = 1)
        {
            var controller = new ShoppingListController(context);
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Role, "Senior")
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
        public void MyLists_Returns_ViewResult_With_Model()
        {
            var context = GetInMemoryContext("TestDb_MyLists_" + System.Guid.NewGuid());
            context.ShoppingLists.Add(new ShoppingList { Id = 1, SeniorId = 1, Title = "Test List", IsFinalized = true });
            context.SaveChanges();

            var controller = GetControllerWithContext(context);

            var result = controller.MyLists();

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<ShoppingListStatusViewModel>>(viewResult.Model);
            Assert.Single(model);
            Assert.Equal("Test List", model.First().ShoppingList.Title);
        }

        [Fact]
        public void Create_Get_ReturnsView()
        {
            var context = GetInMemoryContext("CreateGetDb");
            var controller = GetControllerWithContext(context);

            var result = controller.Create();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Items_ReturnsNotFound_WhenListDoesNotExist()
        {
            var context = GetInMemoryContext("ItemsNotFoundDb");
            var controller = GetControllerWithContext(context);

            var result = controller.Items(999); // nieistniejące ID

            Assert.IsType<NotFoundResult>(result);
        }


        [Fact]
        public void Take_AssignsVolunteer_WhenListAvailable()
        {
            var context = GetInMemoryContext("TakeDb");
            var list = new ShoppingList { Id = 1, SeniorId = 1, Title = "Test", VolunteerId = null, Status = "Oczekuje" };
            context.ShoppingLists.Add(list);
            context.SaveChanges();

            var controller = GetControllerWithContext(context);

            var result = controller.Take(1);

            Assert.IsType<RedirectToActionResult>(result);
            var updatedList = context.ShoppingLists.First(l => l.Id == 1);
            Assert.Equal(1, updatedList.VolunteerId); // zakładając, że w kodzie ustawiasz tymczasowo VolunteerId = 1
            Assert.Equal("W realizacji", updatedList.Status);
        }

        [Fact]
        public void RemoveItem_DoesNothing_WhenItemNotFound()
        {
            var context = GetInMemoryContext("RemoveNonexistentItemDb");
            var list = new ShoppingList { Id = 1, SeniorId = 1, Title = "Test List", Items = new List<ShoppingItem>() };
            context.ShoppingLists.Add(list);
            context.SaveChanges();

            var controller = GetControllerWithContext(context);

            var result = controller.RemoveItem(1, 999); // nieistniejący itemId

            var updatedList = context.ShoppingLists.Include(l => l.Items).First();
            Assert.Empty(updatedList.Items); // lista nadal pusta
            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public void Finalize_ReturnsNotFound_WhenListDoesNotExist()
        {
            var context = GetInMemoryContext("FinalizeNonexistentDb");
            var controller = GetControllerWithContext(context);

            var result = controller.Finalize(999); // nieistniejące ID

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Items_ReturnsView_WhenListExists()
        {
            var context = GetInMemoryContext("ItemsExistsDb");
            var list = new ShoppingList { Id = 1, SeniorId = 1, Title = "Test List" };
            context.ShoppingLists.Add(list);
            context.SaveChanges();

            var controller = GetControllerWithContext(context);

            var result = controller.Items(1);

            Assert.IsType<ViewResult>(result);
        }


        [Fact]
        public void Available_ReturnsViewResult()
        {
            var context = GetInMemoryContext("AvailableDb");
            var controller = GetControllerWithContext(context);

            var result = controller.Available();

            Assert.IsType<ViewResult>(result);
        }
    }
}
