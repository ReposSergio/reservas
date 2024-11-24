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
        private readonly ILogger<AuthenticationController> _logger; 

    
        public AuthenticationController(IJwtTokenService jwtTokenService, ILogger<AuthenticationController> logger)
        {
            _jwtTokenService = jwtTokenService;
            _logger = logger;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest loginRequest)
        {
            if (loginRequest == null || string.IsNullOrEmpty(loginRequest.Email) || string.IsNullOrEmpty(loginRequest.Password))
            {
                return BadRequest("Email y contraseña son requeridos.");
            }

            try
            {
                
                var client = GetClientByEmailAndPassword(loginRequest.Email, loginRequest.Password); 
                if (client == null)
                {
                    return Unauthorized(new { Message = "Invalid credentials" });
                }

                
                var token = _jwtTokenService.GenerateToken(client);

                
                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                
                _logger.LogError("Error generating token: {Error}", ex.ToString());
                return StatusCode(500, new { message = "An unexpected error occurred. Please try again later.", details = ex.Message });
            }
        }

        
        private Client GetClientByEmailAndPassword(string email, string password)
        {            
            return new Client { Email = email, Name = "Test Client", Id = 1 };  
        }
    }
}
