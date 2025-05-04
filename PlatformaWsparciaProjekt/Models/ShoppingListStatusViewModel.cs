using System;

namespace PlatformaWsparciaProjekt.Models
{
    public class ShoppingListStatusViewModel
    {
        public ShoppingList ShoppingList { get; set; }  // ← NAJWAŻNIEJSZE
        public string Status { get; set; }              // np. "W realizacji", "Oczekuje"
    }
}




