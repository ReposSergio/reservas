using Microsoft.AspNetCore.Mvc;
using BookingSystem.Data;
using BookingSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BookingSystem.Controllers
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

        // Crear una nueva reserva
        [HttpPost]
        public async Task<IActionResult> CreateReservation([FromBody] CreateReservationDto reservationDto)
        {
            try
            {
                // Verificar que el cliente exista
                var client = await _context.Clients.FindAsync(reservationDto.ClientId);
                if (client == null)
                {
                    _logger.LogWarning("El cliente con ID {ClientId} no existe.", reservationDto.ClientId);
                    return BadRequest("El cliente no existe.");
                }

                // Verificar que el servicio exista
                var service = await _context.Services.FindAsync(reservationDto.ServiceId);
                if (service == null)
                {
                    _logger.LogWarning("El servicio con ID {ServiceId} no existe.", reservationDto.ServiceId);
                    return BadRequest("El servicio no existe.");
                }

                // Convertir la fecha a UTC
                var reservationDateUtc = reservationDto.ReservationDate.ToUniversalTime();

                // Crear la reserva
                var reservation = new Reservation
                {
                    ClientId = reservationDto.ClientId,
                    ServiceId = reservationDto.ServiceId,
                    ReservationDate = reservationDateUtc,  // Usar la fecha en UTC
                    Notes = reservationDto.Notes
                };

                // Agregar la reserva al contexto y guardar los cambios
                _context.Reservations.Add(reservation);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Reserva creada con éxito para el cliente {ClientId} con el servicio {ServiceId}.", reservationDto.ClientId, reservationDto.ServiceId);

                // Retornar la respuesta con el objeto creado
                return CreatedAtAction(nameof(GetReservation), new { id = reservation.Id }, reservation);
            }
            catch (Exception ex)
            {
                // Registrar el error completo en caso de excepciones
                _logger.LogError(ex, "Ocurrió un error al crear la reserva.");
                return StatusCode(500, "Hubo un error al crear la reserva. Intente nuevamente.");
            }
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

        // Filtrar reservas por cliente, servicio o fecha
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
