namespace InventoryManagement.API.DTOs
{
    public class PermissionDto
    {
        public string PermissionName { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }
}