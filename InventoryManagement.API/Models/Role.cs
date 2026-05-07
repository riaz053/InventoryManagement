namespace InventoryManagement.API.Models
{
    public class Role
    {
        public int Id { get; set; }
        public string rName { get; set; }

        public ICollection<UserRole> UserRoles { get; set; }
    }
}