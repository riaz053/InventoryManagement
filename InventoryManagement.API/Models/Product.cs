using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagement.API.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string ProductCode { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string ProductName { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal PurchasePrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal SalesPrice { get; set; }

        public int ReorderLevel { get; set; } = 0;

        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        public int UnitId { get; set; }
        public Units Unit { get; set; } = null!;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}

// namespace InventoryManagement.API.Models
// {
//     public class Product
//     {
//         public int Id { get; set; }
//         public string pCode { get; set; } = string.Empty;
//         public string pName { get; set; } = string.Empty;

//         public int CategoryId { get; set; }
//         public Category Category { get; set; }

//         public int UnitId { get; set; }
//         public Units Unit { get; set; }

//         public decimal Price { get; set; }
//         public bool IsActive { get; set; } = true;
//     }
// }