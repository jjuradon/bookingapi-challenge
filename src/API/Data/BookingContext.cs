using Booking.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Booking.API.Data
{
    public class BookingContext : DbContext
    {
        public BookingContext(DbContextOptions<BookingContext> options) : base(options)
        {
        }

        public DbSet<Reservation> Reservations { get; set; }
    }
}