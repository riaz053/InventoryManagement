namespace InventoryManagement.API.DTOs
{
    public class MenuViewDto
    {
        public int MenuId { get; set; }

        public string MenuName { get; set; } = string.Empty;

        public string? MenuCode { get; set; }

        public string? Link { get; set; }

        public string? Icon { get; set; }

        public int DisplayOrder { get; set; }

        public bool IsActive { get; set; }

        public bool IsGroup { get; set; }

        public int? ParentMenuId { get; set; }

        public string? ParentMenuName { get; set; }
    }
}