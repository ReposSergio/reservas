namespace BookingSystem.Models
{
    public class Service
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }

        public List<Reservation> Reservations { get; set; } = new List<Reservation>();
        
    }
}
