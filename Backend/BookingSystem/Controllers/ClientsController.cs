using Microsoft.AspNetCore.Mvc;
using BookingSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

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
        public async Task<IActionResult> CreateClient([FromBody] CreateClientRequest model)
        {
            // Validar la entrada
            if (model == null)
            {
                return BadRequest("El cuerpo de la solicitud es inválido.");
            }

            // Generar un "salt" único para el usuario
            var salt = GenerateSalt();

            // Hashear la contraseña con PBKDF2 y el salt generado
            var passwordHash = HashPassword(model.Password, salt);

            // Crear el cliente con la contraseña hasheada
            var client = new Client
            {
                Name = model.Name,
                Email = model.Email,
                PasswordHash = passwordHash,
                Salt = salt // Guardar el salt para futuras comparaciones
            };

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

        // Método privado para verificar si un cliente existe por ID
        private bool ClientExists(int id)
        {
            return _context.Clients.Any(e => e.Id == id);
        }

        // Método para generar un salt aleatorio
        private string GenerateSalt()
        {
            byte[] saltBytes = new byte[128 / 8]; // 128 bits de salt
            System.Security.Cryptography.RandomNumberGenerator.Fill(saltBytes);
            return Convert.ToBase64String(saltBytes);
        }

        // Método para hacer el hash de la contraseña
        private string HashPassword(string password, string salt)
        {
            var hash = KeyDerivation.Pbkdf2(
                password: password,
                salt: Convert.FromBase64String(salt), 
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000, 
                numBytesRequested: 32 
            );

            return Convert.ToBase64String(hash);
        }
    }

    // Modelo para registrar un cliente (sin la contraseña hasheada ni el salt)
    public class CreateClientRequest
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
