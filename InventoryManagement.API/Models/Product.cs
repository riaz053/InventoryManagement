namespace InventoryManagement.API.Models
{
    public class Product
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Code { get; set; } = string.Empty;

        public decimal PurchaseRate { get; set; }

        public decimal SaleRate { get; set; }

        public int Quantity { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}