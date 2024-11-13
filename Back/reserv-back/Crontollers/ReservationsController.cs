using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookingReservations.Data;
using BookingReservations.Models;
using Microsoft.Extensions.Logging;

namespace BookingReservations.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationsController : ControllerBase
    {
        private readonly BookingDbContext _context;
        private readonly ILogger<ReservationsController> _logger;

        public ReservationsController(BookingDbContext context, ILogger<ReservationsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Endpoint para crear una nueva reserva
        [HttpPost]
        public async Task<ActionResult<Reservation>> CreateReservation(Reservation reservation)
        {
            try
            {
                _context.Reservations.Add(reservation);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetReservation), new { id = reservation.Id }, reservation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear una reserva.");
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        // Endpoint para obtener una reserva por ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Reservation>> GetReservation(int id)
        {
            var reservation = await _context.Reservations
                .Include(r => r.Client)
                .Include(r => r.Service)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reservation == null)
            {
                return NotFound();
            }

            return reservation;
        }

        // Endpoint para actualizar una reserva
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReservation(int id, Reservation reservation)
        {
            if (id != reservation.Id)
            {
                return BadRequest("El ID de la reserva no coincide.");
            }

            _context.Entry(reservation).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Reservations.Any(r => r.Id == id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // Endpoint para eliminar una reserva
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReservation(int id)
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

        // Endpoint para obtener reservas con filtrado
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Reservation>>> GetReservations([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate, [FromQuery] int? clientId, [FromQuery] int? serviceId)
        {
            var query = _context.Reservations.Include(r => r.Client).Include(r => r.Service).AsQueryable();

            if (startDate.HasValue)
                query = query.Where(r => r.Date >= startDate.Value);
            if (endDate.HasValue)
                query = query.Where(r => r.Date <= endDate.Value);
            if (clientId.HasValue)
                query = query.Where(r => r.ClientId == clientId);
            if (serviceId.HasValue)
                query = query.Where(r => r.ServiceId == serviceId);

            return await query.ToListAsync();
        }
    }
}
