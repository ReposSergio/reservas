using BookingSystem.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using BookingSystem.Interfaces;
using BookingSystem.Services;
using System.Text;
using BookingSystem.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Leer configuración de JwtSettings
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"];
var issuer = jwtSettings["Issuer"];
var audience = jwtSettings["Audience"];

if (string.IsNullOrEmpty(secretKey))
{
    throw new ArgumentNullException("JwtSettings:SecretKey", "La clave secreta de JWT no está configurada.");
}

// Mostrar configuración para depuración
Console.WriteLine($"Jwt Secret Key: {secretKey}");
Console.WriteLine($"Issuer: {issuer}");
Console.WriteLine($"Audience: {audience}");
Console.WriteLine($"Connection String: {builder.Configuration.GetConnectionString("BookingDb")}");

// Registrar servicios
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

// Configurar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost3000", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Configurar la cadena de conexión a PostgreSQL
builder.Services.AddDbContext<BookingDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("BookingDb")));

// Configurar autenticación JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
        };
    });

// Configurar JSON en controladores
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

// Agregar servicios para Swagger y logging
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddLogging();

var app = builder.Build();

// Configurar middleware personalizado de manejo de errores
app.UseMiddleware<ErrorHandlingMiddleware>();

// Configurar CORS
app.UseCors("AllowLocalhost3000");

// Configurar Swagger
app.UseSwagger();
app.UseSwaggerUI();

// Configurar autenticación y autorización
app.UseAuthentication();
app.UseAuthorization();

// Configurar rutas de controladores
app.MapControllers();

app.Run();
