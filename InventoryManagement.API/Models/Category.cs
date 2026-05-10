namespace InventoryManagement.API.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string catCode { get; set; } = string.Empty;
        public string catName { get; set; } = string.Empty;
    }
}