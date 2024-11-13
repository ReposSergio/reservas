using Microsoft.EntityFrameworkCore;
using BookingReservations.Models;

namespace BookingReservations.Data
{
    public class BookingDbContext : DbContext
    {
        public BookingDbContext(DbContextOptions<BookingDbContext> options) : base(options) { }

        public DbSet<Reservation>? Reservations { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Service> Services { get; set; }
    }
}
