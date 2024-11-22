using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using BookingReservationsAPI.Data;
using BookingReservationsAPI.Models;

namespace BookingReservationsAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public LoginController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // POST api/login
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest loginRequest)
        {
            if (loginRequest == null || string.IsNullOrEmpty(loginRequest.Email) || string.IsNullOrEmpty(loginRequest.Password))
            {
                return BadRequest("Correo electrónico y contraseña son obligatorios.");
            }

            // Buscar al customer por correo electrónico
            var customer = _context.Customers.FirstOrDefault(c => c.Email == loginRequest.Email);
            if (customer == null)
            {
                Console.WriteLine($"No se encontró un cliente con el correo: {loginRequest.Email}");
                return Unauthorized("Correo electrónico o contraseña inválidos.");
            }

            // Agregar log para depuración: muestra el hash de la contraseña almacenada
            Console.WriteLine($"Hash de la contraseña almacenada: {customer.PasswordHash}");

            // Agregar log para depuración: muestra la contraseña ingresada
            Console.WriteLine($"Contraseña ingresada: {loginRequest.Password}");

            // Verificar si la contraseña proporcionada coincide con el hash almacenado
            bool isPasswordValid = customer.VerifyPassword(loginRequest.Password);

            // Agregar log para depuración: muestra si la contraseña es válida
            Console.WriteLine($"¿La contraseña es válida? {isPasswordValid}");

            if (!isPasswordValid)
            {
                return Unauthorized("Correo electrónico o contraseña inválidos.");
            }

            // Generar el token JWT si la contraseña es válida
            var token = GenerateJwtToken(customer);
            return Ok(new { token });
        }

        // Método para generar el token JWT
        private string GenerateJwtToken(Customer customer)
        {
            var claims = new[] 
            {
                new Claim(ClaimTypes.NameIdentifier, customer.Id.ToString()),
                new Claim(ClaimTypes.Name, customer.Name),
                new Claim(ClaimTypes.Email, customer.Email)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    // Clase para la solicitud de inicio de sesión
    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
