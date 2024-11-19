using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookingReservationsAPI.Data;
using BookingReservationsAPI.Models;

namespace BookingReservationsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BookingsController(AppDbContext context)
        {
            _context = context;
        }

        // Crear una nueva reserva
        [HttpPost]
        public async Task<ActionResult<Booking>> CreateBooking([FromBody] Booking booking)
        {

            if (booking == null)
            {
                return BadRequest("Reserva no válida.");
            }

            booking.ReservationDate = DateTime.SpecifyKind(booking.ReservationDate, DateTimeKind.Utc);

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBooking), new { id = booking.Id }, booking);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Booking>>> GetBookings()
        {
            var bookings = await _context.Bookings.ToListAsync();

            if (bookings == null || bookings.Count == 0)
            {
                return NotFound("No se encontraron reservas.");
            }

            return Ok(bookings);
        }



        [HttpGet("{id}")]
        public async Task<ActionResult<Booking>> GetBooking(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }

            return booking;
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound($"No se encontró la reserva con ID {id}.");
            }

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();
            return NoContent(); // 204 No Content: indica que la operación fue exitosa, pero no hay contenido que devolver
        }

         [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBooking(int id, [FromBody] Booking updatedBooking)
        {
            if (id != updatedBooking.Id)
            {
                return BadRequest("El ID de la reserva no coincide.");
            }

            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound($"No se encontró la reserva con ID {id}.");
            }

            // Actualizar los valores de la reserva
            booking.CustomerId = updatedBooking.CustomerId;
            booking.ServiceId = updatedBooking.ServiceId;
            booking.ReservationDate = updatedBooking.ReservationDate;
            booking.Notes = updatedBooking.Notes;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookingExists(id))
                {
                    return NotFound($"No se encontró la reserva con ID {id}.");
                }
                else
                {
                    throw;
                }
            }

            return NoContent(); // 204 No Content: indica que la operación fue exitosa
        }


        private bool BookingExists(int id)
        {
            return _context.Bookings.Any(b => b.Id == id);
        }
    }
}
