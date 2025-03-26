namespace PlatformaWsparciaProjekt.Models
{
    public class ShoppingList
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Senior, który stworzył listę
        public int SeniorId { get; set; }
        public Senior Senior { get; set; }

        // Lista pozycji na zakupy
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
