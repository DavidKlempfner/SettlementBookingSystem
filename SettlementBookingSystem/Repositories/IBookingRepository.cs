using SettlementBookingSystem.Models;

namespace SettlementBookingSystem.Repositories
{
    public interface IBookingRepository
    {
        void CreateBooking(BookingResult booking);

        IReadOnlyList<BookingResult> GetBookings();
    }
}
