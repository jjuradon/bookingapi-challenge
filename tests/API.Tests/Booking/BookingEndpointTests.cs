using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Booking.API.Tests.Base;
using Booking.API.Tests.Fixture;
using Booking.API.Tests.Utils;
using Booking.Core.Domain.Entities;
using Booking.Core.Models;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace Booking.API.Tests.Booking
{
    public class BookingEndpointTests : EndpointTestsBase, IDisposable
    {
        public BookingEndpointTests(BookingApiFactory factory) : base(factory)
        {
        }

        private const string D_FORMAT = "yyyy-MM-dd";

        public override string ENDPOINT => "booking";

        [Fact]
        public async Task Post_PlaceReservationFor1Day_Response201Created()
        {
            //Given
            var date = Utilities.GetStringDate(1);
            var body = new 
            {
                Date = date
            };
            //When
            var response = await VerifySuccessResponse<ReservationResponseModel>(TestMethods.POST, StatusCodes.Status201Created, body);
            //Then
            Assert.Equal(date, response.EndDate);
        }

        [Fact]
        public async Task Post_PlaceReservationForReservedDay_Response409Conflict()
        {
            //Given
            var date = Utilities.GetStringDate(16);
            var body = new 
            {
                Date = date,
                Days = 3
            };
            //When
            var response = await VerifyErrorResponse(TestMethods.POST, StatusCodes.Status409Conflict, body);
            //Then
            ErrorResponse(response, "The room is already reserved for that period.");
        }

        [Theory]
        [InlineData(1)]
        [InlineData(6)]
        [InlineData(10)]
        public async Task Post_CheckAvailabeFor1Day_Response200Ok(int day)
        {
            //Given
            var date = Utilities.GetStringDate(day);
            var body = new
            {
                Date = date
            };
            //When
            var response = await VerifySuccessResponse<IDictionary<string, string>>(TestMethods.POST, StatusCodes.Status200OK, body, $"check");
            //Then
            Assert.Equal("The room is available.", response["message"]);
        }

        [Theory]
        [InlineData(5, 2)]
        [InlineData(14, 3)]
        [InlineData(30, 3)]
        public async Task Post_CheckAvailableForMultipleDays_Response200Ok(int day, int days)
        {
            //Given
            var date = Utilities.GetStringDate(day);
            var body = new
            {
                Date = date,
                Days = days
            };
            //When
            var response = await VerifySuccessResponse<IDictionary<string, string>>(TestMethods.POST, StatusCodes.Status200OK, body, $"check");
            //Then
            Assert.Equal("The room is available.", response["message"]);
        }

        [Theory]
        [InlineData(3)]
        [InlineData(8)]
        [InlineData(17)]
        public async Task Post_CheckNotAvailabeFor1Day_Response409Conflict(int day)
        {
            //Given
            var date = Utilities.GetStringDate(day);
            var body = new
            {
                Date = date
            };
            //When
            var response = await VerifyErrorResponse(TestMethods.POST, StatusCodes.Status409Conflict, body, "check");
            //Then
            ErrorResponse(response, "The room is already reserved for that period.");
        }

        [Theory]
        [InlineData(1, 2)]
        [InlineData(4, 3)]
        [InlineData(13, 2)]
        [InlineData(16, 3)]
        [InlineData(16, 2)]
        public async Task Post_CheckNotAvailableForMultipleDays_Response409Conflict(int day, int days)
        {
            //Given
            var date = Utilities.GetStringDate(day);
            var body = new
            {
                Date = date,
                Days = days
            };
            //When
            var response = await VerifyErrorResponse(TestMethods.POST, StatusCodes.Status409Conflict, body, "check");
            //Then
            ErrorResponse(response, "The room is already reserved for that period.");
        }

        [Fact]
        public async Task Delete_CancelAnExistingReservation_Response204NoContent()
        {
            //Given
            var entity = _db.InsertReservation(new Reservation
            {
                Start = Utilities.GetDate(24),
                Days = 1,
                Finish = Utilities.GetDate(24)
            });
            //Then
            await VerifySuccessResponse(TestMethods.DELETE, StatusCodes.Status204NoContent, entity.Id);
        }

        [Fact]
        public async Task Delete_CancelNotExistingReservation_Response404NotFound()
        {
            //Given
            var id = 20;
            //When
            var response = await VerifyErrorResponse(TestMethods.DELETE, StatusCodes.Status404NotFound, id);
            //Then
            NotFoundErrorResponse(response, "Reservation");
        }

        [Fact]
        public async Task Put_ModifyReservationToMultipleSameDate_Response200Ok()
        {
            //Given
            var entity = _db.InsertReservation(new Reservation
            {
                Start = Utilities.GetDate(24),
                Days = 1,
                Finish = Utilities.GetDate(24)
            });
            var date = Utilities.GetStringDate(24);
            var body = new 
            {
                Date = date,
                Days = 3
            };
            //When
            var response = await VerifySuccessResponse<ReservationResponseModel>(TestMethods.PUT, StatusCodes.Status200OK, body, entity.Id);
            //Then
            Assert.Equal(Utilities.GetStringDate(26), response.EndDate);
        }

        [Fact]
        public async Task Put_ModifyNotExistingReservation_Response404NotFound()
        {
            //Given
            var id = 20;
            var date = Utilities.GetStringDate(16);
            var body = new 
            {
                Date = date,
                Days = 3
            };
            //When
            var response = await VerifyErrorResponse(TestMethods.PUT, StatusCodes.Status404NotFound, body, id);
            //Then
            NotFoundErrorResponse(response, "Reservation");
        }

        [Fact]
        public async Task Put_ModifyReservationTo1DaySameDate_Response200Ok()
        {
            //Given
            var entity = _db.InsertReservation(new Reservation
            {
                Start = Utilities.GetDate(21),
                Days = 2,
                Finish = Utilities.GetDate(22)
            });
            var date = Utilities.GetStringDate(21);
            var body = new 
            {
                Date = date
            };
            //When
            var response = await VerifySuccessResponse<ReservationResponseModel>(TestMethods.PUT, StatusCodes.Status200OK, body, entity.Id);
            //Then
            Assert.Equal(Utilities.GetStringDate(21), response.EndDate);
        }

        [Fact]
        public async Task Put_ModifyReservationToMultipleDayWithReservedDate_Response409Conflict()
        {
            //Given
            var entity = _db.InsertReservation(new Reservation
            {
                Start = Utilities.GetDate(16),
                Days = 1,
                Finish = Utilities.GetDate(16)
            });
            var date = Utilities.GetStringDate(16);
            var body = new 
            {
                Date = date,
                Days = 3
            };
            //When
            var response = await VerifyErrorResponse(TestMethods.PUT, StatusCodes.Status409Conflict, body, entity.Id);
            //Then
            ErrorResponse(response, "The room is already reserved for that period.");
        }

        [Fact]
        public async Task Put_ModifyReservationAlreadyActive_Response409Conflict()
        {
            //Given
            // var entity = _db.InsertReservation(new Reservation
            // {
            //     Start = Utilities.GetDate(0),
            //     Days = 1,
            //     Finish = Utilities.GetDate(16)
            // });
            var date = Utilities.GetStringDate(16);
            var body = new 
            {
                Date = date,
                Days = 3
            };
            //When
            var response = await VerifyErrorResponse(TestMethods.PUT, StatusCodes.Status409Conflict, body, 1);
            //Then
            ErrorResponse(response, $"The reservation cannot be deleted or modified. Started {Utilities.GetStringDate(-2)}");
        }

        public void Dispose()
        {
            _db.ReInitializeDbForTests();
        }
    }
}