using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using BookingReservationsAPI.Data;
using System.Security.Cryptography;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Verificar si el programa se ejecuta con el argumento "generate-key"
if (args.Contains("generate-key"))
{
    GenerateAndPrintKey();
    return; // Terminar el programa después de generar la clave
}

// Configuración de la base de datos Postgres
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configuración de JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var key = builder.Configuration["Jwt:Key"];
        var keyBytes = EnsureValidKeySize(key); // Validar y ajustar el tamaño de la clave

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(keyBytes) 
        };
    });

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost3000", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowLocalhost3000");

app.UseHttpsRedirection();

// Agregar autenticación y autorización
app.UseAuthentication(); // Habilitar autenticación JWT
app.UseAuthorization();

app.MapControllers();
app.Run();


// Método para generar una clave aleatoria y mostrarla en consola
static void GenerateAndPrintKey()
{
    using (var rng = RandomNumberGenerator.Create())
    {
        byte[] key = new byte[64]; // Tamaño de 64 bytes
        rng.GetBytes(key);
        Console.WriteLine("Nueva clave generada:");
        Console.WriteLine(Convert.ToBase64String(key)); // Imprimir en Base64
    }
}

// Método para asegurar que la clave tiene al menos 32 bytes
static byte[] EnsureValidKeySize(string key)
{
    var keyBytes = Convert.FromBase64String(key); // Usar base64 directamente

    if (keyBytes.Length < 32)
    {
        // Si la clave es más corta que 32 bytes, la ajustamos
        var newKey = new byte[32];
        Array.Copy(keyBytes, newKey, Math.Min(keyBytes.Length, 32));
        return newKey;
    }
    return keyBytes; // Si ya tiene 32 bytes, no se cambia
}
