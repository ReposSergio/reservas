namespace BookingReservationsAPI.Models
{
    public class Customer
    {
        public int Id { get; set; }

        public required string Name { get; set; }

        public required string Email { get; set; }  

        
        public required string PasswordHash { get; set; }

        
        public void SetPassword(string password)
        {
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
        }

        
        public bool VerifyPassword(string password)
        {
            return BCrypt.Net.BCrypt.Verify(password, PasswordHash);
        }
    }
}
