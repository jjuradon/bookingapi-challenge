namespace Booking.Core.Models
{
    public class ReservationResponseModel : ReservationModel
    {
        public int Id { get; set; }
        public string EndDate { get; set; }
    }
}