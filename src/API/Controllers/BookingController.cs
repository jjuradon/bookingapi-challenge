using System.Collections.Generic;
using System.Threading.Tasks;
using Booking.Core.Interfaces;
using Booking.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace Booking.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _service;

        public BookingController(IBookingService service)
        {
            _service = service;
        }

        [HttpPost("check")]
        public async Task<IActionResult> CheckAvailability([FromBody] ReservationModel model)
        {
            await _service.CheckAvailability(model);
            return Ok(new Dictionary<string, string> { {"message", "The room is available."} });
        }

        [HttpPost]
        public async Task<IActionResult> PlaceReservation([FromBody] ReservationModel model)
        {
            var response = await _service.PlaceReservation(model);
            return CreatedAtAction(nameof(CheckAvailability), new { date = response.Date }, response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> CancelReservation([FromRoute] int id)
        {
            await _service.CancelReservation(id);
            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ModifyReservation([FromRoute] int id, [FromBody] ReservationModel model)
        {
            var response = await _service.UpdateReservation(id, model);
            return Ok(response);
        }
    }
}