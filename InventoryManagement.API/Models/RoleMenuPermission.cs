namespace InventoryManagement.API.Models
{
    public class RoleMenuPermission
    {
        public int RoleMenuPermissionId { get; set; }

        public int RoleId { get; set; }

        public int MenuId { get; set; }

        public int PermissionId { get; set; }

        public bool IsAllowed { get; set; } = true;

        public ApplicationRole Role { get; set; } = null!;
        public Menu Menu { get; set; } = null!;
        public Permission Permission { get; set; } = null!;
    }
}