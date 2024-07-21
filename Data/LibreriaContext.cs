using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

using Microsoft.EntityFrameworkCore;
using Libreria.Models;

namespace Libreria.Data
{
    public class LibreriaContext : IdentityDbContext<User, Role, string>
    {
        public LibreriaContext(DbContextOptions<LibreriaContext> options)
            : base(options)
        {
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<DiscountConfig> DiscountConfigs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de relaciones y restricciones
            modelBuilder.Entity<Order>()
                .HasMany(o => o.OrderDetails)
                .WithOne(od => od.Order)
                .HasForeignKey(od => od.OrderId);

            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Book)
                .WithMany()
                .HasForeignKey(od => od.BookId);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Customer)
                .WithMany(c => c.PurchaseHistory)
                .HasForeignKey(o => o.CustomerId);

            // Seed data para DiscountConfig
            modelBuilder.Entity<DiscountConfig>().HasData(
                new DiscountConfig { Id = 1, DiscountPercentage = 10.0M } // Ejemplo de configuración de descuento
            );

            // Seed data para el administrador
            var adminId = "admin-id";
            var adminRoleId = "admin-role-id";
            modelBuilder.Entity<Role>().HasData(new Role { Id = adminRoleId, Name = "Administrator", NormalizedName = "ADMINISTRATOR" });

            modelBuilder.Entity<User>().HasData(new User
            {
                Id = adminId,
                UserName = "admin",
                NormalizedUserName = "ADMIN",
                PasswordHash = new PasswordHasher<User>().HashPassword(null, "adminpassword"), 
                Rut = "admin-rut",
                FullName = "Admin User",
                Address = "Admin Address",
                DateOfBirth = DateTime.Now,
                PhoneNumber = "123456789",
                Email = "admin@example.com"
            });

            modelBuilder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string> { RoleId = adminRoleId, UserId = adminId });
        }
    }
}
