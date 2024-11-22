namespace BookingSystem.Models
{
    public class Client
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        // Relación: Un cliente puede tener muchas reservas, pero no es obligatorio incluirlas al crear un cliente
        public List<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}
