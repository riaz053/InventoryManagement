using Microsoft.AspNetCore.Identity;

namespace InventoryManagement.API.Models
{
    // Custom role class for ASP.NET Core Identity
    public class ApplicationRole : IdentityRole<int>
    {
        // Optional extra fields
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
    }
}