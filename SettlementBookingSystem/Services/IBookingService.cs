using SettlementBookingSystem.Models;

namespace SettlementBookingSystem.Services
{
    public interface IBookingService
    {
        BookingResult CreateBooking(Booking booking);
    }
}