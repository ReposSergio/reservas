using Microsoft.EntityFrameworkCore;
using BookingReservations.Data;

var builder = WebApplication.CreateBuilder(args);

// Agregar servicios al contenedor.
builder.Services.AddControllers();

// Configurar la cadena de conexión a la base de datos.
builder.Services.AddDbContext<BookingDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Agregar el servicio de registro (Logging).
builder.Services.AddLogging();

var app = builder.Build();

// Configurar la canalización HTTP.
app.UseHttpsRedirection();
app.MapControllers();

app.Run();
