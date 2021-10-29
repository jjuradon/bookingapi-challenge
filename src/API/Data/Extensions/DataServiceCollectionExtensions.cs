using System;
using Booking.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Booking.API.Data
{
    public static class DataServiceCollectionExtensions
    {
        /// <summary>
        /// Registers the given context as a service in the Microsoft.Extensions.DependencyInjection.IServiceCollection . 
        /// You use this method when using dependency injection in your application, such as with ASP.NET. 
        /// The register uses SqlServer initializing with a connectionstring with the context name.
        /// </summary>
        /// <param name="services">The IServiceCollection to add services to.</param>
        /// <param name="Configuration">The Iconfiguration to retrieve a connection string from app settings.</param>
        /// <returns>The same service collection so that multiple calls can be chained.</returns>
        public static IServiceCollection AddSqlContext(this IServiceCollection services, IConfiguration Configuration)
        {
            var conString = Configuration.GetConnectionString(typeof(BookingContext).Name);
            if (!string.IsNullOrEmpty(conString))
                services.AddDbContext<BookingContext>(options =>
                options.UseSqlServer(conString));
            else
                throw new InvalidOperationException("Couldn't connect to database. DB Context connection string not found");
            
            services.AddScoped<IReservationRepository, ReservationQueries>();
            return services;
        }
    }
}