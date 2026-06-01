namespace InventoryManagement.API.DTOs
{
    public class MenuTreeDto
    {
        public int MenuId { get; set; }

        public string MenuName { get; set; } = string.Empty;

        public string? Link { get; set; }

        public string? Icon { get; set; }

        public int? ParentMenuId { get; set; }

        public List<MenuTreeDto> Children { get; set; }
            = new List<MenuTreeDto>();
    }
}