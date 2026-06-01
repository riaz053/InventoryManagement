namespace InventoryManagement.API.Models
{
    public class Menu
    {
        public int MenuId { get; set; }

        public string MenuCode { get; set; } = string.Empty;

        public string MenuName { get; set; } = string.Empty;

        public string? Link { get; set; }

        public string? Icon { get; set; }

        public int DisplayOrder { get; set; }

        public int? ParentMenuId { get; set; }

        public Menu? ParentMenu { get; set; }

        public ICollection<Menu> Children { get; set; } = new List<Menu>();

        public bool IsActive { get; set; } = true;

        // 🔥 REQUIRED for RoleMenu mapping
        public ICollection<RoleMenu> RoleMenus { get; set; } = new List<RoleMenu>();
    }
}

// namespace InventoryManagement.API.Models
// {
//     public class Menu
//     {
//         public int MenuId { get; set; }

//         // Optional but useful for tracking
//         public string MenuCode { get; set; } = string.Empty;

//         public string MenuName { get; set; } = string.Empty;

//         // URL to navigate (ONLY for clickable menus)
//         public string? Link { get; set; }

//         public string? Icon { get; set; }

//         public int DisplayOrder { get; set; }

//         // 🔥 CORE OF TREE STRUCTURE
//         public int? ParentMenuId { get; set; }
//         public Menu? ParentMenu { get; set; }

//         public ICollection<Menu> Children { get; set; } = new List<Menu>();

//         // Active/Inactive control
//         public bool IsActive { get; set; } = true;
//     }
// }