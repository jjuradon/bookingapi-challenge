using Microsoft.AspNetCore.Builder;

namespace Booking.API.Middleware
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseErrorHandling(this IApplicationBuilder app)
                => app.UseMiddleware<ErrorHandlingMiddleware>();
    }
}