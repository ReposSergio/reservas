using System.ComponentModel.DataAnnotations.Schema;

namespace BookingReservationsAPI.Models
{
    public class Booking
    {
        public int Id { get; set; } 
        public int CustomerId { get; set; } 
        public int ServiceId { get; set; } 

        [Column(TypeName = "timestamp with time zone")]
        public DateTime ReservationDate { get; set; } = DateTime.UtcNow;
        public required string Notes { get; set; }
    }
}