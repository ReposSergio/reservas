using Microsoft.AspNetCore.Mvc;
using BookingSystem.Models;
using BookingSystem.Interfaces;
using Microsoft.Extensions.Logging; 

namespace BookingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IJwtTokenService _jwtTokenService;
        private readonly ILogger<AuthenticationController> _logger; // Logger para registrar los errores

        // Constructor
        public AuthenticationController(IJwtTokenService jwtTokenService, ILogger<AuthenticationController> logger)
        {
            _jwtTokenService = jwtTokenService;
            _logger = logger;
        }

        // Método de login
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest loginRequest)
        {
            if (loginRequest == null || string.IsNullOrEmpty(loginRequest.Email) || string.IsNullOrEmpty(loginRequest.Password))
            {
                return BadRequest("Email y contraseña son requeridos.");
            }

            try
            {
                // Aquí llamas al servicio que genera el token JWT
                var client = GetClientByEmailAndPassword(loginRequest.Email, loginRequest.Password); // Suponiendo que esta es una función que valida el cliente
                if (client == null)
                {
                    return Unauthorized(new { Message = "Invalid credentials" });
                }

                // Generar el token con el servicio
                var token = _jwtTokenService.GenerateToken(client);

                // Retornar el token en la respuesta
                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                // Si ocurre un error, lo registras y devuelves una respuesta de error
                _logger.LogError("Error generating token: {Error}", ex.ToString());
                return StatusCode(500, new { message = "An unexpected error occurred. Please try again later.", details = ex.Message });
            }
        }

        // Función ejemplo para obtener el cliente (debes implementarla según tu lógica)
        private Client GetClientByEmailAndPassword(string email, string password)
        {
            // Aquí buscarías al cliente en la base de datos y validarías la contraseña
            // Esta es solo una muestra simplificada, la lógica de validación depende de tu implementación
            return new Client { Email = email, Name = "Test Client", Id = 1 }; // Cliente de prueba
        }
    }
}
