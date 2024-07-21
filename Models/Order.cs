namespace Libreria.Models;

public class Order
{
    public int Id { get; set; }
    public int? CustomerId { get; set; }
    public Customer Customer { get; set; }
    public DateTime Date { get; set; }
    public int Total { get; set; } // Cambiado a entero
    public ICollection<OrderDetail> OrderDetails { get; set; }
}
