using BookingSystem.Data;
using BookingSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Logging;
using System.Text;
using BookingSystem.Middleware;  // Asegúrate de agregar el espacio de nombres para el middleware

var builder = WebApplication.CreateBuilder(args);

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

// Configuración de JWT Authentication
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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

// Agregar servicios de controlador y swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Agregar logging
builder.Services.AddLogging();

var app = builder.Build();

// Configurar middleware de manejo de errores (ErrorHandlingMiddleware)
app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseCors("AllowLocalhost3000");

// Configurar Swagger
app.UseSwagger();
app.UseSwaggerUI();

// Autenticación y autorización
app.UseAuthentication();
app.UseAuthorization();

// Configurar las rutas de los controladores
app.MapControllers();

app.Run();
