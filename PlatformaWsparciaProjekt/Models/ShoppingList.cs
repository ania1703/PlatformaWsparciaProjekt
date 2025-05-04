namespace PlatformaWsparciaProjekt.Models
{
    public class ShoppingList
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public int SeniorId { get; set; }
        public Senior Senior { get; set; }

        public int? VolunteerId { get; set; }
        public Volunteer? Volunteer { get; set; }

        public string Status { get; set; } = "Oczekuje"; // "Oczekuje", "W realizacji", "Zrealizowana"

        public List<ShoppingItem> Items { get; set; } = new List<ShoppingItem>();
    }

    public class ShoppingItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public bool IsPurchased { get; set; }
        public int ShoppingListId { get; set; }
        public ShoppingList ShoppingList { get; set; }
    }

}
