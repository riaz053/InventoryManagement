namespace InventoryManagement.API.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string pCode { get; set; } = string.Empty;
        public string pName { get; set; } = string.Empty;

        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public int UnitId { get; set; }
        public Units Unit { get; set; }

        public decimal Price { get; set; }
        public bool IsActive { get; set; } = true;
    }
}