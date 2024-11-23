using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;

namespace BookingSystem.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Continuar con la ejecución de la solicitud
                await _next(context);
            }
            catch (Exception ex)
            {
                // Capturar cualquier excepción que ocurra
                _logger.LogError($"Something went wrong: {ex.Message}", ex);

                // Establecer el código de estado HTTP
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";

                // Crear la respuesta de error
                var errorResponse = new
                {
                    Message = "An unexpected error occurred. Please try again later.",
                    // En producción, puedes optar por no incluir los detalles del error
                    Details = context.Request.IsDevelopment() ? ex.ToString() : "An unexpected error occurred."
                };

                // Escribir la respuesta JSON
                await context.Response.WriteAsJsonAsync(errorResponse);
            }
        }
    }

    // Agregar un método de extensión para detectar si estamos en el entorno de desarrollo
    public static class HttpContextExtensions
    {
        public static bool IsDevelopment(this HttpRequest request)
        {
            var environment = request.HttpContext.RequestServices.GetService(typeof(IHostEnvironment)) as IHostEnvironment;
            return environment?.IsDevelopment() ?? false;
        }
    }
}
