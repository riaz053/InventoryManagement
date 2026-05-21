namespace InventoryManagement.API.DTOs
{
    public class RoleMenuSaveDto
    {
        public int RoleId { get; set; }

        public List<RoleMenuItemDto> Menus { get; set; }
            = new();
    }

    public class RoleMenuItemDto
    {
        public int MenuId { get; set; }

        public bool IsAllowed { get; set; }
    }
}