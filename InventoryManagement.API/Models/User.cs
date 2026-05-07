namespace InventoryManagement.API.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsActive { get; set; }

        public ICollection<UserRoles> UserRoles { get; set; }
    }
}