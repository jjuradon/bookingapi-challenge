using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Booking.Core.Models
{
    /// <summary>
    /// Class that represent the model base for the controllers.
    /// </summary>
    /// 
    public class ReservationModel : IValidatableObject
    {
        /// <summary>
        /// Staying days in the room.
        /// </summary>
        public int Days { get; set; } = 1;
        /// <summary>
        /// Date for the reservation.
        /// </summary>
        public string Date { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var dFormat = "yyyy-MM-dd";
            var dateRegexString = @"^(19|20)\d\d[-](0[1-9]|1[012])[-](0[1-9]|[12][0-9]|3[01])$";
            var dateRegex = new Regex(dateRegexString);
            var result = new List<ValidationResult>();
            var dNow = DateTime.UtcNow.Date;
            
            if (Days < 1 || Days > 3)
                result.Add(new ValidationResult("Must be between 1 and 3.", new List<string> { nameof(Days) }));
            if (string.IsNullOrEmpty(Date) || string.IsNullOrWhiteSpace(Date))
                result.Add(new ValidationResult("Date is required.", new List<string> { nameof(Date) }));
            else if (!dateRegex.IsMatch(Date))
                result.Add(new ValidationResult($"Invalid format. Must be a date in {dFormat} format.", new List<string> { nameof(Date) }));
            else 
            {
                var d = DateTime.Parse(Date);
                if (d > dNow.AddDays(30))
                    result.Add(new ValidationResult($"Cannot be more than 30 days in advance. {dNow.AddDays(30).ToString(dFormat)}.", new List<string> { nameof(Date) }));
                if (d <= dNow)
                    result.Add(new ValidationResult($"Must be at least 1 more day than today. {dNow.ToString(dFormat)}.", new List<string> { nameof(Date) }));
            }
            return result;
        }
    }
}