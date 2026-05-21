namespace InventoryManagement.API.DTOs
{
    public class PermissionDto
    {
        public int PermissionId { get; set; }

        public string PermissionName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

    }
}