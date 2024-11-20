using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookingReservationsAPI.Data;
using BookingReservationsAPI.Models;

namespace BookingReservationsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ServiceController(AppDbContext context)
        {
            _context = context;
        }

        // Crear un nuevo servicio
        [HttpPost]
        public async Task<ActionResult<Service>> CreateService([FromBody] Service service)
        {
            if (service == null || string.IsNullOrWhiteSpace(service.Name))
            {
                return BadRequest("El servicio no es válido.");
            }

            _context.Services.Add(service);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetService), new { id = service.Id }, service);
        }

        // Obtener todos los servicios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Service>>> GetServices()
        {
            var services = await _context.Services.ToListAsync();

            if (services == null || services.Count == 0)
            {
                return NotFound("No se encontraron servicios.");
            }

            return Ok(services);
        }

        // Obtener un servicio por ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Service>> GetService(int id)
        {
            var service = await _context.Services.FindAsync(id);

            if (service == null)
            {
                return NotFound($"No se encontró el servicio con ID {id}.");
            }

            return Ok(service);
        }

        // Actualizar un servicio existente
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateService(int id, [FromBody] Service updatedService)
        {
            if (id != updatedService.Id)
            {
                return BadRequest("El ID del servicio no coincide.");
            }

            var service = await _context.Services.FindAsync(id);
            if (service == null)
            {
                return NotFound($"No se encontró el servicio con ID {id}.");
            }

            // Actualizar las propiedades del servicio
            service.Name = updatedService.Name;
            service.Description = updatedService.Description;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ServiceExists(id))
                {
                    return NotFound($"No se encontró el servicio con ID {id}.");
                }
                else
                {
                    throw;
                }
            }

            return NoContent(); // 204 No Content
        }

        // Eliminar un servicio
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteService(int id)
        {
            var service = await _context.Services.FindAsync(id);
            if (service == null)
            {
                return NotFound($"No se encontró el servicio con ID {id}.");
            }

            _context.Services.Remove(service);
            await _context.SaveChangesAsync();
            return NoContent(); // 204 No Content
        }

        // Verificar si un servicio existe
        private bool ServiceExists(int id)
        {
            return _context.Services.Any(s => s.Id == id);
        }
    }
}
