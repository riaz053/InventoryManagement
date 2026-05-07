namespace InventoryManagement.API.DTOs
{
    public class CreateUserDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int RoleId { get; set; }
    }
}