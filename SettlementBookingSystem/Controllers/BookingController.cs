using Microsoft.AspNetCore.Mvc;
using SettlementBookingSystem.Models;
using SettlementBookingSystem.Services;

namespace SettlementBookingSystem.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BookingController(IBookingService bookingService) : ControllerBase
    {
        [HttpPost]
        public BookingResult CreateBooking(Booking booking)
        {
            return bookingService.CreateBooking(booking);
        }
    }
}