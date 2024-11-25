using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> CreateReservation([FromBody] CreateReservationRequest model)
        {
            // Verificar si el cliente existe
            var client = await _context.Clients.FindAsync(model.ClientId);
            if (client == null)
            {
                return BadRequest("El cliente no existe.");
            }

            // Verificar si el servicio existe (si es necesario)
            var service = await _context.Services.FindAsync(model.ServiceId);
            if (service == null)
            {
                return BadRequest("El servicio no existe.");
            }

            // Convertir la fecha al formato UTC si no lo está
            if (model.ReservationDate.Kind != DateTimeKind.Utc)
            {
                model.ReservationDate = DateTime.SpecifyKind(model.ReservationDate, DateTimeKind.Utc);
            }

            var reservation = new Reservation
            {
                ClientId = model.ClientId,
                ServiceId = model.ServiceId,
                ReservationDate = model.ReservationDate,
                Notes = model.Notes
            };

            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetReservation), new { id = reservation.Id }, reservation);
        }

        // Obtener una reserva por ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetReservation(int id)
        {
            // Cargar la reserva junto con los datos del cliente y servicio relacionados
            var reservation = await _context.Reservations
                .Include(r => r.Client)   // Incluye el cliente relacionado
                .Include(r => r.Service)  // Incluye el servicio relacionado
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reservation == null)
            {
                return NotFound("La reserva no existe.");
            }

            return Ok(reservation);
        }


        // Obtener todas las reservas de un cliente con filtros opcionales
        // Obtener todas las reservas de un cliente con filtros opcionales
        [HttpGet("client/{clientId}")]
        public async Task<ActionResult<IEnumerable<Reservation>>> GetReservationsByClientId(
            int clientId,
            [FromQuery] int? serviceId,
            [FromQuery] DateTime? date)
        {
            var query = _context.Reservations.AsQueryable();

            // Filtrar por clientId
            query = query.Where(r => r.ClientId == clientId);

            // Filtros adicionales
            if (serviceId.HasValue)
            {
                query = query.Where(r => r.ServiceId == serviceId);
            }

            if (date.HasValue)
            {
                query = query.Where(r => r.ReservationDate.Date == date.Value.Date);
            }

            // Devolver las reservas filtradas
            return await query.ToListAsync();
        }

        // Modificar una reserva existente
        [HttpPut("{id}")]
        public async Task<IActionResult> PutReservation(int id, [FromBody] Reservation reservation)
        {
            if (id != reservation.Id)
            {
                return BadRequest("El ID de la reserva no coincide.");
            }

            // Verificar si la reserva existe en la base de datos
            var existingReservation = await _context.Reservations
                .Include(r => r.Client)  // Incluir la entidad relacionada Client
                .Include(r => r.Service)  // Incluir la entidad relacionada Service
                .FirstOrDefaultAsync(r => r.Id == id);

            if (existingReservation == null)
            {
                return NotFound("La reserva no existe.");
            }

            // Validar que los datos de ClientId y ServiceId son válidos
            if (reservation.ClientId <= 0 || reservation.ServiceId <= 0)
            {
                return BadRequest("El ID del cliente y el ID del servicio deben ser válidos.");
            }

            // Actualizar los campos de la reserva existente
            existingReservation.ClientId = reservation.ClientId;
            existingReservation.ServiceId = reservation.ServiceId;
            existingReservation.ReservationDate = reservation.ReservationDate;
            existingReservation.Notes = reservation.Notes;

            // Guardar los cambios en la base de datos
            try
            {
                _context.Entry(existingReservation).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return NoContent(); // Se devuelve 204 si la operación fue exitosa y no se necesita respuesta.
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Reservations.Any(r => r.Id == id))
                {
                    return NotFound("La reserva no se pudo actualizar porque no existe.");
                }
                throw;
            }
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

        // Obtener todas las reservas con filtros opcionales
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

        // Método privado para verificar si una reserva existe por ID
        private bool ReservationExists(int id)
        {
            return _context.Reservations.Any(e => e.Id == id);
        }
    }

    // Modelo para la creación de una reserva
    public class CreateReservationRequest
    {
        public int ClientId { get; set; }
        public int ServiceId { get; set; }
        public DateTime ReservationDate { get; set; }
        public string Notes { get; set; }
    }
}
