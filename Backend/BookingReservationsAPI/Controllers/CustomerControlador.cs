using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookingReservationsAPI.Data;
using BookingReservationsAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Text.RegularExpressions;
using BCrypt.Net;  // Asegúrate de haber instalado el paquete BCrypt.Net-Next

namespace BookingReservationsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CustomerController(AppDbContext context)
        {
            _context = context;
        }

        // Crear un nuevo cliente
        [HttpPost]
        public async Task<ActionResult<Customer>> CreateCustomer([FromBody] Customer customer)
        {
            if (customer == null || string.IsNullOrWhiteSpace(customer.Name) || string.IsNullOrWhiteSpace(customer.Email) || string.IsNullOrWhiteSpace(customer.PasswordHash))
            {
                return BadRequest("El cliente no es válido. Asegúrate de que los campos Nombre, Email y Contraseña no estén vacíos.");
            }

            // Validar formato del correo electrónico
            if (!IsValidEmail(customer.Email))
            {
                return BadRequest("El correo electrónico no tiene un formato válido.");
            }

            // Hashear la contraseña antes de guardarla
            customer.SetPassword(customer.PasswordHash); // Hasheando la contraseña

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCustomer), new { id = customer.Id }, customer);
        }

        // Obtener todos los clientes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
        {
            var customers = await _context.Customers.ToListAsync();

            if (customers == null || customers.Count == 0)
            {
                return NotFound("No se encontraron clientes.");
            }

            return Ok(customers);
        }

        // Obtener un cliente por ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
            {
                return NotFound($"No se encontró el cliente con ID {id}.");
            }

            return Ok(customer);
        }

        // Actualizar un cliente existente
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomer(int id, [FromBody] Customer updatedCustomer)
        {
            if (id != updatedCustomer.Id)
            {
                return BadRequest("El ID del cliente no coincide.");
            }

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound($"No se encontró el cliente con ID {id}.");
            }

            // Actualizar las propiedades del cliente
            customer.Name = updatedCustomer.Name;
            customer.Email = updatedCustomer.Email;  // Actualizar Email

            // Si se proporciona una nueva contraseña, hashearla y actualizarla
            if (!string.IsNullOrWhiteSpace(updatedCustomer.PasswordHash)) 
            {
                customer.SetPassword(updatedCustomer.PasswordHash); // Hashear nueva contraseña
            }

            // Validar el formato del correo electrónico
            if (!IsValidEmail(customer.Email))
            {
                return BadRequest("El correo electrónico no tiene un formato válido.");
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(id))
                {
                    return NotFound($"No se encontró el cliente con ID {id}.");
                }
                else
                {
                    throw;
                }
            }

            return NoContent(); // 204 No Content
        }

        // Eliminar un cliente
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound($"No se encontró el cliente con ID {id}.");
            }

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
            return NoContent(); // 204 No Content
        }

        // Verificar si un cliente existe
        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(c => c.Id == id);
        }

        // Función para validar el formato del email
        private bool IsValidEmail(string email)
        {
            var emailRegex = new Regex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");
            return emailRegex.IsMatch(email);
        }
    }
}
