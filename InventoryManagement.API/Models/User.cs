namespace InventoryManagement.API.Models
{
    public class User
    {
        public int Id { get; set; }

        public string UserCode { get; set; } = string.Empty;

        public string Username { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public ICollection<UserRoles> UserRoles { get; set; } = new List<UserRoles>();
    }
}