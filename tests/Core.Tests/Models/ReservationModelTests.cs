using System;
using System.Linq;
using Booking.Core.Models;
using Xunit;

namespace Booking.Core.Tests.Models
{
    public class ReservationModelClass
    {
        [Fact]
        public void Validate_DaysOutOfValues_ResturnsValidationResultList()
        {
            //Given
            var model = new ReservationModel 
            {
                Days = 0,
                Date = GetDate()
            };
            //When
            var result = model.Validate(null);
            //Then
            Assert.NotEmpty(result);
            Assert.Collection(result, 
                item => {
                    Assert.Equal("Must be between 1 and 3.", item.ErrorMessage);
                    Assert.Equal("Days", item.MemberNames.First());
                    });
        }

        [Theory]
        [InlineData("something")]
        [InlineData("12-12-2020")]
        [InlineData("2020 12 12")]
        [InlineData("2020/12/12")]
        public void Validate_DateInvalidFormat_ResturnValidationResultList(string date)
        {
            //Given
            var model = new ReservationModel 
            {
                Date = date
            };
            //When
            var result = model.Validate(null);
            //Then
            Assert.NotEmpty(result);
            Assert.Collection(result, 
                item => {
                    Assert.Equal("Invalid format. Must be a date in yyyy-MM-dd format.", item.ErrorMessage);
                    Assert.Equal("Date", item.MemberNames.First());
                    });
        }

        [Theory]
        [InlineData(" ")]
        [InlineData(null)]
        public void Validate_RequiredPropertyMissing_ResturnValidationResultList(string date)
        {
            //Given
            var model = new ReservationModel 
            {
                Date = date
            };
            //When
            var result = model.Validate(null);
            //Then
            Assert.NotEmpty(result);
            Assert.Collection(result, 
                item => {
                    Assert.Equal("Date is required.", item.ErrorMessage);
                    Assert.Equal("Date", item.MemberNames.First());
                    });
        }

        [Fact]
        public void Validate_DateIsMoreThan30DaysInAdvance_ReturnsValidationResultList()
        {
            //Given
            var date = GetDate(31);
            var model = new ReservationModel
            {
                Date = date
            };
            //When
            var result = model.Validate(null);
            //Then
            Assert.NotEmpty(result);
            Assert.Collection(result, 
                item => {
                    Assert.Equal($"Cannot be more than 30 days in advance. {GetDate(30)}.", item.ErrorMessage);
                    Assert.Equal("Date", item.MemberNames.First());
                    });
        }

        [Fact]
        public void Validate_DateIsEqualToToday_ReturnsValidationResultList()
        {
            //Given
            var date = GetDate(0);
            var model = new ReservationModel
            {
                Date = date
            };
            //When
            var result = model.Validate(null);
            //Then
            Assert.NotEmpty(result);
            Assert.Collection(result, 
                item => {
                    Assert.Equal($"Must be at least 1 more day than today. {GetDate(0)}.", item.ErrorMessage);
                    Assert.Equal("Date", item.MemberNames.First());
                    });
        }

        private string GetDate(int days = 1) => DateTime.UtcNow.AddDays(days).ToString("yyyy-MM-dd");
    }
    
}