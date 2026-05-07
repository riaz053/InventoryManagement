using InventoryManagement.API.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options
        ) : base(options)
        {
        }

        // Tables
        public DbSet<User> Users { get; set; }

        public DbSet<Role> Roles { get; set; }

        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed Roles
            modelBuilder.Entity<Role>().HasData(
                new Role
                {
                    Id = 1,
                    RoleName = "Admin"
                },
                new Role
                {
                    Id = 2,
                    RoleName = "ProductUser"
                },
                new Role
                {
                    Id = 3,
                    RoleName = "SalesUser"
                }
            );
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