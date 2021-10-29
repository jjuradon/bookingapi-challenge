using System;
using System.Threading.Tasks;
using Booking.Core.Domain.Entities;
using Booking.Core.Models;

namespace Booking.Core.Interfaces
{
    public interface IReservationRepository : IRepository<Reservation>
    {
        Task<bool> IsAvailableOn(ReservationModel model);
        Task<Reservation> GetById(int id);
        Task<bool> IsAvailableForUpdate(Reservation entity);
    } 
}