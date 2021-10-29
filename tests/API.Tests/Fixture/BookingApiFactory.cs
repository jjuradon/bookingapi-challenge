using System.Linq;
using Booking.API.Data;
using Booking.API.Tests.Utils;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Booking.API.Tests.Fixture
{
    public class BookingApiFactory : WebApplicationFactory<Startup>
    {
        public BookingContext Db { get => Services.GetRequiredService<BookingContext>(); }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Remove the app's ApplicationDbContext registration.
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType ==
                        typeof(DbContextOptions<BookingContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }
                
                // Add ApplicationDbContext using an in-memory database for testing.
                services.AddDbContext<BookingContext>(options =>
                {
                    options.UseInMemoryDatabase("TestingDatabase");
                });

                // Build the service provider.
                var sp = services.BuildServiceProvider();

                // Create a scope to obtain a reference to the database
                // context (ApplicationDbContext).
                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<BookingContext>();
                    // var logger = scopedServices
                    //     .GetRequiredService<ILogger<MeaningfulApiFactory>>();

                    // Ensure the database is created.
                    db.Database.EnsureCreated();
                    // Seed the database with test data.
                    db.InitializeDbForTests();
                }
            }).UseEnvironment("Testing");
            base.ConfigureWebHost(builder);
        }
    }
}