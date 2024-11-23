using Microsoft.AspNetCore.Mvc;
using BookingSystem.Data;
using BookingSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationsController : ControllerBase
    {
        private readonly BookingDbContext _context;

        public ReservationsController(BookingDbContext context)
        {
            _context = context;
        }

        // Crear una nueva reserva
        [HttpPost]
        public async Task<IActionResult> CreateReservation(Reservation reservation)
        {
            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetReservation), new { id = reservation.Id }, reservation);
        }

        // Obtener una reserva por ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Reservation>> GetReservation(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);

            if (reservation == null)
            {
                return NotFound();
            }

            return reservation;
        }

        // Modificar una reserva
        [HttpPut("{id}")]
        public async Task<IActionResult> ModifyReservation(int id, Reservation reservation)
        {
            if (id != reservation.Id)
            {
                return BadRequest();
            }

            _context.Entry(reservation).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReservationExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // Eliminar una reserva
        [HttpDelete("{id}")]
        public async Task<IActionResult> CancelReservation(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }

            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Reservation>>> GetReservations([FromQuery] int? clientId, [FromQuery] int? serviceId, [FromQuery] DateTime? date)
        {
            var query = _context.Reservations.AsQueryable();

            if (clientId.HasValue)
            {
                query = query.Where(r => r.ClientId == clientId);
            }

            if (serviceId.HasValue)
            {
                query = query.Where(r => r.ServiceId == serviceId);
            }

            if (date.HasValue)
            {
                query = query.Where(r => r.ReservationDate.Date == date.Value.Date);
            }

            return await query.ToListAsync();
        }

        private bool ReservationExists(int id)
        {
            return _context.Reservations.Any(e => e.Id == id);
        }
    }
}
