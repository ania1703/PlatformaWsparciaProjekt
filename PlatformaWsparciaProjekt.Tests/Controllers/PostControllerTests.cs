using Xunit;
using PlatformaWsparciaProjekt.Controllers;
using PlatformaWsparciaProjekt.Data;
using PlatformaWsparciaProjekt.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Moq;

namespace PlatformaWsparciaProjekt.Tests.Controllers
{
    public class PostControllerTests
    {
        private AppDbContext GetInMemoryContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new AppDbContext(options);
        }

        private PostController GetController(AppDbContext context)
        {
            var mockEnv = new Mock<IWebHostEnvironment>();
            mockEnv.Setup(e => e.WebRootPath).Returns("wwwroot");
            return new PostController(context, mockEnv.Object);
        }

        [Fact]
        public void Create_Get_Returns_View()
        {
            var context = GetInMemoryContext("PostCreateGetDb");
            var controller = GetController(context);

            var result = controller.Create();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Edit_Get_Returns_NotFound_When_Id_NotExist()
        {
            var context = GetInMemoryContext("PostEditDb");
            var controller = GetController(context);

            var result = controller.Edit(999); // brakujący ID

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Create_Post_InvalidModel_ReturnsView()
        {
            var context = GetInMemoryContext("PostCreateInvalidDb");
            var controller = GetController(context);
            controller.ModelState.AddModelError("Content", "Required");

            var model = new PlatformaWsparciaProjekt.Models.PostViewModel();

            var result = await controller.Create(model);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.False(viewResult.ViewData.ModelState.IsValid);
        }

        [Fact]
        public void Edit_Get_Returns_Forbid_When_UserMismatch()
        {
            var context = GetInMemoryContext("PostEditForbidDb");
            context.Posts.Add(new PlatformaWsparciaProjekt.Models.Post
            {
                Id = 1,
                UserId = 1,
                Role = "Senior",
                Content = "Test",
                CreatedAt = DateTime.Now
            });
            context.SaveChanges();

            var controller = GetController(context);
            // symulujemy innego użytkownika
            var user = new System.Security.Claims.ClaimsPrincipal(
                new System.Security.Claims.ClaimsIdentity(new[]
                {
            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, "2"),
            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, "Senior")
                }, "mock"));
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext { User = user }
            };

            var result = controller.Edit(1);

            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task Delete_Returns_Forbid_When_UserMismatch()
        {
            var context = GetInMemoryContext("PostDeleteForbidDb");
            context.Posts.Add(new PlatformaWsparciaProjekt.Models.Post
            {
                Id = 1,
                UserId = 1,
                Role = "Senior",
                Content = "Test",
                CreatedAt = DateTime.Now
            });
            context.SaveChanges();

            var controller = GetController(context);
            var user = new System.Security.Claims.ClaimsPrincipal(
                new System.Security.Claims.ClaimsIdentity(new[]
                {
            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, "2"),
            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, "Senior")
                }, "mock"));
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext { User = user }
            };

            var result = await controller.Delete(1);

            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public void Edit_Get_Returns_View_When_Ok()
        {
            var context = GetInMemoryContext("PostEditOkDb");
            context.Posts.Add(new PlatformaWsparciaProjekt.Models.Post
            {
                Id = 1,
                UserId = 1,
                Role = "Senior",
                Content = "Test",
                CreatedAt = DateTime.Now
            });
            context.SaveChanges();

            var controller = GetController(context);
            var user = new System.Security.Claims.ClaimsPrincipal(
                new System.Security.Claims.ClaimsIdentity(new[]
                {
            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, "1"),
            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, "Senior")
                }, "mock"));
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext { User = user }
            };

            var result = controller.Edit(1);

            Assert.IsType<ViewResult>(result);
        }


        [Fact]
        public async Task Delete_Returns_NotFound_When_Id_NotExist()
        {
            var context = GetInMemoryContext("PostDeleteDb");
            var controller = GetController(context);

            var result = await controller.Delete(999); // brakujący ID

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
