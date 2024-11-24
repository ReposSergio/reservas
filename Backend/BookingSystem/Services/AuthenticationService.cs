using Microsoft.AspNetCore.Identity;
using BookingSystem.Models;
using BookingSystem.Interfaces;



public class AuthenticationService
{
    private readonly UserManager<Client> _userManager;
    private readonly IJwtTokenService _jwtTokenService;

    public AuthenticationService(UserManager<Client> userManager, IJwtTokenService jwtTokenService)
    {
        _userManager = userManager;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<string> AuthenticateAsync(string email, string password)
    {
        var client = await _userManager.FindByEmailAsync(email);
        if (client == null)
        {
            throw new UnauthorizedAccessException("El cliente no existe.");
        }

        // Comprobar si la contraseña es correcta
        bool isPasswordValid = await _userManager.CheckPasswordAsync(client, password);
        if (!isPasswordValid)
        {
            throw new UnauthorizedAccessException("Contraseña incorrecta.");
        }

        // Generar un token JWT
        var token = _jwtTokenService.GenerateToken(client);
        return token;
    }
}
