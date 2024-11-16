using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class ReservationController : ControllerBase
{
    private readonly BookingContext _context;

    public ReservationController(BookingContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> CreateReservation(Reservation reservation)
    {
        _context.Reservations.Add(reservation);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetReservation), new { id = reservation.Id }, reservation);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateReservation(int id, Reservation updatedReservation)
    {
        var reservation = await _context.Reservations.FindAsync(id);
        if (reservation == null) return NotFound();

        reservation.Date = updatedReservation.Date;
        reservation.CustomerId = updatedReservation.CustomerId;
        reservation.ServiceId = updatedReservation.ServiceId;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> CancelReservation(int id)
    {
        var reservation = await _context.Reservations.FindAsync(id);
        if (reservation == null) return NotFound();

        _context.Reservations.Remove(reservation);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet]
    public async Task<IActionResult> GetReservations([FromQuery] DateTime? date, [FromQuery] int? serviceId, [FromQuery] int? customerId)
    {
        var query = _context.Reservations.Include(r => r.Customer).Include(r => r.Service).AsQueryable();

        if (date.HasValue)
            query = query.Where(r => r.Date.Date == date.Value.Date);
        if (serviceId.HasValue)
            query = query.Where(r => r.ServiceId == serviceId.Value);
        if (customerId.HasValue)
            query = query.Where(r => r.CustomerId == customerId.Value);

        return Ok(await query.ToListAsync());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetReservation(int id)
    {
        var reservation = await _context.Reservations.Include(r => r.Customer).Include(r => r.Service).FirstOrDefaultAsync(r => r.Id == id);
        if (reservation == null) return NotFound();
        return Ok(reservation);
    }
}
