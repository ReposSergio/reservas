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
                _logger.LogError($"Something went wrong: {ex.Message}");

                // Configurar la respuesta de error
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";

                var errorResponse = new
                {
                    Message = "An unexpected error occurred. Please try again later.",
                    Details = ex.Message // Puedes incluir más detalles o personalizar según tu necesidad
                };

                await context.Response.WriteAsJsonAsync(errorResponse);
            }
        }
    }
}
