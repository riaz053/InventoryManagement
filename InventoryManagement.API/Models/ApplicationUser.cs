using Microsoft.AspNetCore.Identity;

namespace InventoryManagement.API.Models
{
    // Custom user class for ASP.NET Core Identity
    public class ApplicationUser : IdentityUser<int>
    {
        public string FullName { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }
}