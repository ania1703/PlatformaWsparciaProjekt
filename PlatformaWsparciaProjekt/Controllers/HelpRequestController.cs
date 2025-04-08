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

        // WYŚWIETLANIE LISTY ZGŁOSZEŃ
        public IActionResult Index()
        {
            var requests = _context.HelpRequests.ToList();
            return View(requests);
        }

        // DODAWANIE NOWEGO ZGŁOSZENIA
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

                Console.WriteLine("Dodano zgłoszenie: " + request.Title); // tymczasowe logowanie

                return RedirectToAction("Index");
            }
            return View(request);
        }


        // EDYCJA ZGŁOSZENIA
        public IActionResult Edit(int id)
        {
            var request = _context.HelpRequests.Find(id);
            if (request == null)
            {
                return NotFound();
            }
            return View(request);
        }

        [HttpPost]
        public IActionResult Edit(HelpRequest request)
        {
            if (ModelState.IsValid)
            {
                _context.HelpRequests.Update(request);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(request);
        }

        // USUWANIE ZGŁOSZENIA
        public IActionResult Delete(int id)
        {
            var request = _context.HelpRequests.Find(id);
            if (request == null)
            {
                return NotFound();
            }
            return View(request);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var request = _context.HelpRequests.Find(id);
            if (request != null)
            {
                _context.HelpRequests.Remove(request);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
