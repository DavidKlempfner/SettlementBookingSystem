using SettlementBookingSystem.Models;
using System.Collections.Immutable;

namespace SettlementBookingSystem.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly List<BookingResult> _bookings = [];

        public void CreateBooking(BookingResult booking)
        {
            _bookings.Add(booking);
        }

        public IReadOnlyList<BookingResult> GetBookings()
        {
            return _bookings.OrderBy(x => x.Booking.BookingTime).ToImmutableList();
        }
    }
}