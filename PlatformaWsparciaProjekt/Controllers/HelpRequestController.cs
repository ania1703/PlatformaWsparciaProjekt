using Microsoft.AspNetCore.Mvc;
using PlatformaWsparciaProjekt.Data;
using PlatformaWsparciaProjekt.Models;

namespace PlatformaWsparciaProjekt.Controllers
{
    public class HelpRequestController : Controller
    {
        private readonly AppDbContext _context;

        public HelpRequestController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var requests = _context.HelpRequests.ToList();
            return View(requests);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(HelpRequest request)
        {
            if (ModelState.IsValid)
            {
                _context.HelpRequests.Add(request);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(request);
        }
    }
}