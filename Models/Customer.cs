namespace Libreria.Models;

public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Rut { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public bool DiscountApplied { get; set; }
    public decimal DiscountPercentage { get; set; } // Descuento aplicado
    public ICollection<Order> PurchaseHistory { get; set; }
}
