using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace PdfApi.Web.Infrastructure.Middleware
{
    public class ExceptionHandling
    {
        private readonly RequestDelegate _next;

        public ExceptionHandling(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                httpContext.Response.Clear();
                httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                httpContext.Response.ContentType = "text/plain";

                await httpContext.Response.WriteAsync(ex.Message);
            }
        }
    }

    public static class ExceptionHandlingExtensions
    {
        public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandling>();
        }
    }
}
