public class Reservation
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public int CustomerId { get; set; }
    public Customer Customer { get; set; }
    public int ServiceId { get; set; }
    public Service Service { get; set; }
}

public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public ICollection<Reservation> Reservations { get; set; }
}

public class Service
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public ICollection<Reservation> Reservations { get; set; }
}
