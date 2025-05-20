using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlatformaWsparciaProjekt.Data;
using PlatformaWsparciaProjekt.Models;
using System.Linq;
using System.Security.Claims;

namespace PlatformaWsparciaProjekt.Controllers
{
    public class ShoppingListController : Controller
    {
        private readonly AppDbContext _context;

        public ShoppingListController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(ShoppingList shoppingList)
        {
            shoppingList.SeniorId = 1; // Tymczasowo, później z sesji
            shoppingList.Status = "Oczekuje";
            shoppingList.CreatedAt = DateTime.Now;

            _context.ShoppingLists.Add(shoppingList);
            _context.SaveChanges();
            TempData["SuccessMessage"] = "Lista zakupów została utworzona.";
            return RedirectToAction("Items", new { id = shoppingList.Id });
        }

        public IActionResult MyLists()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var lists = _context.ShoppingLists
                .Where(l => l.SeniorId == userId)
                .Include(l => l.Items)
                .ToList();

            var helpRequests = _context.HelpRequests
                .Where(hr => hr.SeniorId == userId && hr.ShoppingListId != null)
                .ToList();

            var listStatuses = lists.Select(list =>
            {
                var matchingRequest = helpRequests.FirstOrDefault(hr => hr.ShoppingListId == list.Id);
                var isInProgress = matchingRequest?.VolunteerId != null;

                string status;
                if (!list.IsFinalized)
                    status = "W trakcie tworzenia";
                else
                    status = isInProgress ? "W realizacji" : "Oczekuje";

                return new ShoppingListStatusViewModel
                {
                    ShoppingList = list,
                    Status = status
                };
            }).ToList();

            return View(listStatuses);
        }


        public IActionResult Available()
        {
            var lists = _context.ShoppingLists
                        .Include(l => l.Senior)
                        .Where(l => l.VolunteerId == null && l.Status == "Oczekuje")
                        .ToList();

            return View(lists);
        }

        public IActionResult Take(int id)
        {
            var list = _context.ShoppingLists.Find(id);
            if (list != null && list.VolunteerId == null)
            {
                list.VolunteerId = 1; // Tymczasowo
                list.Status = "W realizacji";
                _context.SaveChanges();
            }

            return RedirectToAction("Available");
        }

        public IActionResult Items(int id)
        {
            var list = _context.ShoppingLists
                .Include(l => l.Items)
                .FirstOrDefault(l => l.Id == id);

            if (list == null) return NotFound();

            return View(list);
        }

        [HttpPost]
        public IActionResult AddItem(int listId, string itemName, int quantity)
        {
            var list = _context.ShoppingLists
                .Include(l => l.Items)
                .FirstOrDefault(l => l.Id == listId);

            if (list == null) return NotFound();

            list.Items.Add(new ShoppingItem
            {
                Name = itemName,
                Quantity = quantity,
                IsPurchased = false
            });

            _context.SaveChanges();
            TempData["SuccessMessage"] = "Produkt został dodany do listy.";
            return RedirectToAction("Items", new { id = listId });
        }

        [HttpPost]
        public IActionResult RemoveItem(int listId, int itemId)
        {
            var list = _context.ShoppingLists
                .Include(l => l.Items)
                .FirstOrDefault(l => l.Id == listId);

            if (list == null) return NotFound();

            var item = list.Items.FirstOrDefault(i => i.Id == itemId);
            if (item != null)
            {
                list.Items.Remove(item);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Produkt został usunięty z listy.";
            }

            return RedirectToAction("Items", new { id = listId });
        }


        [HttpPost]
        public IActionResult Finalize(int id)
        {
            var list = _context.ShoppingLists.FirstOrDefault(l => l.Id == id);
            if (list == null) return NotFound();

            list.IsFinalized = true;
            _context.SaveChanges();
            TempData["SuccessMessage"] = "Lista zakupów została zakończona.";
            return RedirectToAction("Items", new { id });
        }

    }
}

