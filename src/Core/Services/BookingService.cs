using System;
using System.Linq;
using System.Threading.Tasks;
using Booking.Core.Domain.Entities;
using Booking.Core.Exceptions;
using Booking.Core.Extensions;
using Booking.Core.Interfaces;
using Booking.Core.Models;

namespace Booking.Core.Services
{
    public class BookingService : IBookingService
    {
        private readonly IReservationRepository _repository;

        public BookingService(IReservationRepository repository)
        {
            _repository = repository;
        }

        public async Task CancelReservation(int id)
        {
            var entity = await _repository.GetById(id);
            await _repository.Delete(entity);
        }

        public async Task CheckAvailability(ReservationModel model) 
        { 
            if (!await _repository.IsAvailableOn(model))
                throw new ConflictServiceException("The room is already reserved for that period.");
        }

        public async Task<ReservationResponseModel> PlaceReservation(ReservationModel model)
        {
            await CheckAvailability(model);
            var entity = model.CreateEntity();
            await _repository.Put(entity);
            return entity.ToResponse();
        }

        public async Task<ReservationResponseModel> UpdateReservation(int id, ReservationModel model)
        {
            var entity = await _repository.GetById(id);
            entity.UpdateEntity(model);
            if (!await _repository.IsAvailableForUpdate(entity))
                throw new ConflictServiceException("The room is already reserved for that period.");
            await _repository.Update(entity);
            return entity.ToResponse();
        }
    }
}