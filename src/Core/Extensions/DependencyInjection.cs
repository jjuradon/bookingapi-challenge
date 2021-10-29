using Booking.Core.Interfaces;
using Booking.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Booking.Core.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBookingCore(this IServiceCollection services) => services
            .AddScoped<IBookingService, BookingService>()
            .AddTransient<ITimeService, TimeService>();
    }
}