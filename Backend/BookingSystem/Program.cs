using BookingSystem.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using BookingSystem.Interfaces;
using BookingSystem.Services;
using System.Text;
using BookingSystem.Middleware;  // Middleware personalizado para manejo de errores

var builder = WebApplication.CreateBuilder(args);





Console.WriteLine($"Jwt Key from Configuration: {builder.Configuration["Jwt:Key"]}");
Console.WriteLine($"Connection String: {builder.Configuration.GetConnectionString("BookingDb")}");
// Registrar el servicio de JWT
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();


var secretKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrEmpty(secretKey))
{
    throw new ArgumentNullException("Jwt:Key", "La clave secreta de JWT no está configurada.");
}
Console.WriteLine($"Jwt Key: {secretKey}");

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

// Configurar JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
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
