namespace BookingSystem.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        public int ClientId { get; set; }  // Clave foránea a Client
        public int ServiceId { get; set; } // Clave foránea a Service
        public DateTime ReservationDate { get; set; }
        public string Notes { get; set; }

        // Relaciones
        public Client Client { get; set; }
        public Service Service { get; set; }
    }
}
