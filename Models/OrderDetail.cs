namespace Libreria.Models
{
    public class OrderDetail
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }
        public int BookId { get; set; }
        public Book Book { get; set; }
        public int Quantity { get; set; }
        public int UnitPrice { get; set; } // Cambiado a entero
        public int Subtotal { get; set; } // Cambiado a entero
        public string UserId { get; set; } // ID del vendedor
        public User User { get; set; }
        public int CustomerId { get; set; } // ID del cliente
        public Customer Customer { get; set; }
    }
}
