namespace BookingSystem.Models
{
    public class Client
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
         public string Salt { get; set; }       
        public List<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}
