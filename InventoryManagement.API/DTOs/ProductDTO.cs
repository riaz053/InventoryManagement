public class ProductDto
{
    public int Id { get; set; }
    public string ProductCode { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;

    public decimal PurchasePrice { get; set; }
    public decimal SalesPrice { get; set; }
    public int ReorderLevel { get; set; }

    public int CategoryId { get; set; }
    public int UnitId { get; set; }

    public bool IsActive { get; set; }
}