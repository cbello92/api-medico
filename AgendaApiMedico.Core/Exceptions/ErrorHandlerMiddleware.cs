using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace AgendaApiMedico.Core.Exceptions
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlerMiddleware> _logger;

        public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
        {
            _logger = logger;
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (SqlException error)
            {
                var response = context.Response;
                response.ContentType = "application/json";
                response.StatusCode = (int)HttpStatusCode.BadRequest;

                var result = JsonSerializer.Serialize(new { message = error.Number == 50000 ? error?.Message : "Estamos teniendo inconvenientes, por favor intente nuevamente", statusCode = response.StatusCode });
                _logger.LogError($"Error en {context.Request.Path}: " + error.Message + error.StackTrace + error.TargetSite);
                await response.WriteAsync(result);
            }
            catch (Exception error)
            {
                var response = context.Response;
                response.ContentType = "application/json";
                response.StatusCode = (int)HttpStatusCode.InternalServerError;

                var result = JsonSerializer.Serialize(new { message = error?.Message, statusCode = response.StatusCode });
                _logger.LogError($"Error en {context.Request.Path}: " + error.Message + error.StackTrace + error.TargetSite);
                await response.WriteAsync(result);
            }
        }
    }
}
