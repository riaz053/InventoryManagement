namespace InventoryManagement.API.Models
{
    public class RoleMenu
    {
        public int RoleMenuId { get; set; }

        public int RoleId { get; set; }

        public int MenuId { get; set; }

        public bool IsAllowed { get; set; } = true;

        public ApplicationRole Role { get; set; } = null!;

        public Menu Menu { get; set; } = null!;
    }
}