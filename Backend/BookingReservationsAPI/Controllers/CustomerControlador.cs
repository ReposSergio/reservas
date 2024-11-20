using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookingReservationsAPI.Data;
using BookingReservationsAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

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
            if (customer == null || string.IsNullOrWhiteSpace(customer.Name))
            {
                return BadRequest("El cliente no es válido.");
            }

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
    }
}