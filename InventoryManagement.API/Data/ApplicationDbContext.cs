using Microsoft.EntityFrameworkCore;
using InventoryManagement.API.Models;

namespace InventoryManagement.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRoles> UserRoles { get; set; }
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserRoles>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId);

            modelBuilder.Entity<UserRoles>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();
        }
    }
}

// using Microsoft.EntityFrameworkCore;
// using InventoryManagement.API.Models;

// namespace InventoryManagement.API.Data
// {
//     public class ApplicationDbContext : DbContext
//     {
//         public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
//             : base(options)
//         {
//         }

//         // Tables
//         public DbSet<User> Users { get; set; }
//         public DbSet<Role> Roles { get; set; }
//         public DbSet<Product> Products { get; set; }

//         protected override void OnModelCreating(ModelBuilder modelBuilder)
//         {
//             base.OnModelCreating(modelBuilder);

//             // Seed Roles
//             modelBuilder.Entity<Role>().HasData(
//                 new Role { Id = 1, RoleName = "Admin" },
//                 new Role { Id = 2, RoleName = "ProductUser" },
//                 new Role { Id = 3, RoleName = "SalesUser" }
//             );
//         }
//     }
// }