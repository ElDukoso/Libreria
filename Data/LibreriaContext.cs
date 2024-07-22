using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Libreria.Models;
using System;

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
                
            // Configuración de tipo de columna para propiedades decimal
            modelBuilder.Entity<Customer>()
                .Property(c => c.DiscountPercentage)
                .HasColumnType("decimal(5, 2)"); // Especifica el tipo y precisión

            modelBuilder.Entity<DiscountConfig>()
                .Property(d => d.DiscountPercentage)
                .HasColumnType("decimal(5, 2)"); // Especifica el tipo y precisión

            // Seed data para DiscountConfig
            modelBuilder.Entity<DiscountConfig>().HasData(
                new DiscountConfig { Id = 1, DiscountPercentage = 10.0M } // Ejemplo de configuración de descuento
            );

            // Seed data para roles y usuarios
            var adminId = "admin-id";
            var adminRoleId = "admin-role-id";
            var sellerId = "seller-id";
            var sellerRoleId = "seller-role-id";

            // Seed roles
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = adminRoleId, Name = "Administrator", NormalizedName = "ADMINISTRATOR" },
                new Role { Id = sellerRoleId, Name = "Seller", NormalizedName = "SELLER" }
            );

            // Seed users
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = adminId,
                    UserName = "admin",
                    NormalizedUserName = "ADMIN",
                    PasswordHash = new PasswordHasher<User>().HashPassword(null, "admin123"),
                    Rut = "12345678-9",
                    FullName = "Admin User",
                    Address = "123 Admin St.",
                    DateOfBirth = DateTime.Now.AddYears(-30),
                    PhoneNumber = "123456789",
                    Email = "admin@example.com",
                    NormalizedEmail = "ADMIN@EXAMPLE.COM",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    TwoFactorEnabled = false
                },
                new User
                {
                    Id = sellerId,
                    UserName = "seller",
                    NormalizedUserName = "SELLER",
                    PasswordHash = new PasswordHasher<User>().HashPassword(null, "seller123"),
                    Rut = "98765432-1",
                    FullName = "Seller User",
                    Address = "456 Seller Ave.",
                    DateOfBirth = DateTime.Now.AddYears(-25),
                    PhoneNumber = "987654321",
                    Email = "seller@example.com",
                    NormalizedEmail = "SELLER@EXAMPLE.COM",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    TwoFactorEnabled = false
                }
            );

            // Seed user roles
            modelBuilder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string> { RoleId = adminRoleId, UserId = adminId },
                new IdentityUserRole<string> { RoleId = sellerRoleId, UserId = sellerId }
            );
        }
    }
}
