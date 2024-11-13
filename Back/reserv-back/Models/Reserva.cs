using System;

namespace BookingReservations.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int ServiceId { get; set; }
        public DateTime Date { get; set; }
        public string? Notes { get; set; }

        public required Client Client { get; set; }
        public required Service Service { get; set; }
    }
}
