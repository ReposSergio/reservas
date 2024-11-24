using BookingSystem.Models;
namespace BookingSystem.Interfaces
{
    public interface IJwtTokenService
    {
        string GenerateToken(Client client);
    }
}
