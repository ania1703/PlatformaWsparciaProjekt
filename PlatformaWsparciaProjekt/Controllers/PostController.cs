using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using PlatformaWsparciaProjekt.Data;
using PlatformaWsparciaProjekt.Models;
using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PlatformaWsparciaProjekt.Controllers
{
    [Authorize]
    public class PostController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public PostController(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(PostViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var post = new Post
            {
                Content = model.Content,
                CreatedAt = DateTime.Now,
                Role = User.FindFirst(ClaimTypes.Role)?.Value,
                UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)
            };

            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                var uploadsDir = Path.Combine(_environment.WebRootPath, "uploads");
                Directory.CreateDirectory(uploadsDir);

                var fileName = Path.GetFileName(model.ImageFile.FileName);
                var filePath = Path.Combine(uploadsDir, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ImageFile.CopyToAsync(stream);
                }

                post.ImagePath = "/uploads/" + fileName;
            }

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Post dodany!";
            return RedirectToAction("Index", "Home");
        }


        [HttpGet]
        public IActionResult Edit(int id)
        {
            var post = _context.Posts.FirstOrDefault(p => p.Id == id);

            if (post == null)
                return NotFound();

            // Użytkownik może edytować tylko swój post
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            string role = User.FindFirst(ClaimTypes.Role).Value;

            if (post.UserId != userId || post.Role != role)
                return Forbid();

            return View(post);
        }

        // POST: Post/Edit/5
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Post model, IFormFile imageFile)
        {
            var post = _context.Posts.FirstOrDefault(p => p.Id == id);

            if (post == null)
                return NotFound();

            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            string role = User.FindFirst(ClaimTypes.Role).Value;

            if (post.UserId != userId || post.Role != role)
                return Forbid();

            post.Content = model.Content;
            post.CreatedAt = DateTime.Now;

            if (imageFile != null && imageFile.Length > 0)
            {
                string uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                Directory.CreateDirectory(uploadsFolder);
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                string filePath = Path.Combine(uploadsFolder, fileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await imageFile.CopyToAsync(stream);

                post.ImagePath = "/uploads/" + fileName;
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Post zaktualizowany!";
            return RedirectToAction("Index", "Home");
        }

        // POST: Post/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var post = _context.Posts.FirstOrDefault(p => p.Id == id);

            if (post == null)
                return NotFound();

            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            string role = User.FindFirst(ClaimTypes.Role).Value;

            if (post.UserId != userId || post.Role != role)
                return Forbid();

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Post został usunięty.";
            return RedirectToAction("Index", "Home");
        }
    }
}