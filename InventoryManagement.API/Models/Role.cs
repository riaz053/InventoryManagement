namespace InventoryManagement.API.Models
{
    public class Role
    {
        public int Id { get; set; }
        public string roleCode { get; set; } = string.Empty;
        public string rName { get; set; }

        public ICollection<UserRoles> UserRoles { get; set; }
    }
}