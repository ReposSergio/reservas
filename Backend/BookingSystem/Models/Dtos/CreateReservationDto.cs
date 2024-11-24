public class CreateReservationDto
{
    public int ClientId { get; set; }
    public int ServiceId { get; set; }
    public DateTime ReservationDate { get; set; }
    public string Notes { get; set; }
}
