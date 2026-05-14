namespace InventoryManagement.API.Models
{
    public class UserMenuPermission
    {
        public int UserMenuPermissionId { get; set; }

        public string UserId { get; set; } = string.Empty;

        public int MenuId { get; set; }

        public int PermissionId { get; set; }

        public bool IsAllowed { get; set; }

        public ApplicationUser User { get; set; } = null!;
        public Menu Menu { get; set; } = null!;
        public Permission Permission { get; set; } = null!;
    }
}