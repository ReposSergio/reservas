using Microsoft.AspNetCore.Mvc;
using BookingSystem.Data;
using BookingSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly BookingDbContext _context;

        public ClientsController(BookingDbContext context)
        {
            _context = context;
        }

        // Crear un nuevo cliente
        [HttpPost]
        public async Task<IActionResult> CreateClient(Client client)
        {
            _context.Clients.Add(client);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetClient), new { id = client.Id }, client);
        }

        // Obtener un cliente por ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Client>> GetClient(int id)
        {
            var client = await _context.Clients.FindAsync(id);

            if (client == null)
            {
                return NotFound();
            }

            return client;
        }

        // Modificar un cliente existente
        [HttpPut("{id}")]
        public async Task<IActionResult> ModifyClient(int id, Client client)
        {
            if (id != client.Id)
            {
                return BadRequest();
            }

            _context.Entry(client).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClientExists(id))
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

        // Eliminar un cliente
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClient(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client == null)
            {
                return NotFound();
            }

            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Obtener todos los clientes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Client>>> GetClients()
        {
            return await _context.Clients.ToListAsync();
        }

        private bool ClientExists(int id)
        {
            return _context.Clients.Any(e => e.Id == id);
        }
    }
}
