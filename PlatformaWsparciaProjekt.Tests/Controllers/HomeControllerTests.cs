using Xunit;
using PlatformaWsparciaProjekt.Controllers;
using PlatformaWsparciaProjekt.Data;
using PlatformaWsparciaProjekt.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;

namespace PlatformaWsparciaProjekt.Tests.Controllers
{
    public class HomeControllerTests
    {
        private AppDbContext GetInMemoryContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new AppDbContext(options);
        }

        private ILogger<HomeController> GetLogger()
        {
            return LoggerFactory.Create(builder => builder.AddConsole())
                                .CreateLogger<HomeController>();
        }

        [Fact]
        public void Index_ReturnsView()
        {
            var context = GetInMemoryContext("HomeIndexDb");
            var logger = GetLogger();
            var controller = new HomeController(logger, context);

            var result = controller.Index();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Privacy_ReturnsView()
        {
            var context = GetInMemoryContext("HomePrivacyDb");
            var logger = GetLogger();
            var controller = new HomeController(logger, context);

            var result = controller.Privacy();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Index_ReturnsView_WithOrderedPosts()
        {
            var context = GetInMemoryContext("HomeIndexOrderedDb");

            context.Posts.Add(new Post
            {
                Id = 1,
                Content = "First",
                CreatedAt = DateTime.Now.AddHours(-1),
                Role = "Senior", // dodane!
                UserId = 1        // opcjonalnie
            });
            context.Posts.Add(new Post
            {
                Id = 2,
                Content = "Second",
                CreatedAt = DateTime.Now,
                Role = "Volunteer", // dodane!
                UserId = 2          // opcjonalnie
            });

            context.SaveChanges();

            var logger = GetLogger();
            var controller = new HomeController(logger, context);

            var result = controller.Index();
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<Post>>(viewResult.Model);

            Assert.Equal(2, model[0].Id); // najnowszy jako pierwszy
            Assert.Equal(1, model[1].Id);
        }


        [Fact]
        public void Error_ReturnsViewModel_WhenTraceIdIsNull()
        {
            var context = GetInMemoryContext("HomeErrorNullTrace");
            var logger = GetLogger();
            var controller = new HomeController(logger, context);

            // Symuluj brak TraceIdentifier i brak Activity
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            Activity.Current = null;

            var result = controller.Error();

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ErrorViewModel>(viewResult.Model);
            Assert.NotNull(model.RequestId); // powinno spaść do HttpContext.TraceIdentifier (które może być wygenerowane domyślnie)
        }

        [Fact]
        public void Index_ReturnsEmptyList_WhenNoPosts()
        {
            var context = GetInMemoryContext("HomeIndexEmptyDb");
            var logger = GetLogger();
            var controller = new HomeController(logger, context);

            var result = controller.Index();
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<List<Post>>(viewResult.Model);

            Assert.Empty(model);
        }


        [Fact]
        public void Error_ReturnsView_WithModel()
        {
            var context = GetInMemoryContext("HomeErrorDb");
            var logger = GetLogger();
            var controller = new HomeController(logger, context);

            // Ustawiamy sztuczny HttpContext, żeby miał TraceIdentifier
            controller.ControllerContext.HttpContext = new DefaultHttpContext
            {
                TraceIdentifier = "test-trace-id"
            };

            var result = controller.Error();

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ErrorViewModel>(viewResult.Model);
            Assert.Equal("test-trace-id", model.RequestId);
        }

    }
}
