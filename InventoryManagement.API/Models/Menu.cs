namespace InventoryManagement.API.Models
{
    public class Menu
    {
        public int MenuId { get; set; }

        public string MenuCode { get; set; } = string.Empty;

        public string MenuName { get; set; } = string.Empty;

        // Direct URL to open when clicked
        public string? Link { get; set; }

        // Icon class (optional)
        public string? Icon { get; set; }

        // Sorting order
        public int DisplayOrder { get; set; }

        // Parent menu reference
        public int? ParentMenuId { get; set; }
        public Menu? ParentMenu { get; set; }

        // Child menus
        public ICollection<Menu> Children { get; set; } = new List<Menu>();

        // Folder/group menu
        public bool IsGroup { get; set; } = false;

        // Active/inactive
        public bool IsActive { get; set; } = true;
    }
}