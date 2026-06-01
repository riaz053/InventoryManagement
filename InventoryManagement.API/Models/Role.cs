namespace InventoryManagement.API.Models
{
    public class Role
    {
        public int Id { get; set; }

        public string roleCode { get; set; } = string.Empty;

        public string RoleName { get; set; } = string.Empty;

        public ICollection<UserRoles> UserRoles { get; set; } = new List<UserRoles>();

        public ICollection<RoleMenu> RoleMenus { get; set; } = new List<RoleMenu>();
    }
}