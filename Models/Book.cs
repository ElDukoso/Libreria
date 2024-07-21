namespace Libreria.Models;

public class Book
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    public int Price { get; set; } // Cambiado a entero
    public int Stock { get; set; }
    public string Publisher { get; set; }
    public DateTime PublicationDate { get; set; }
}
