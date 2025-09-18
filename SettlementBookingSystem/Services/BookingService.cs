using SettlementBookingSystem.CustomExceptions;
using SettlementBookingSystem.Models;
using SettlementBookingSystem.Repositories;

namespace SettlementBookingSystem.Services
{
    public class BookingService(IBookingRepository bookingRepository) : IBookingService
    {
        private const int MaxSimultaneousBookings = 4;

        public BookingResult CreateBooking(Booking booking)
        {
            if (!CanCreateBooking(booking.BookingTime))
            {
                throw new BookingConflictException($"Cannot create booking due to maximum ({MaxSimultaneousBookings}) simultaneous bookings reached.");
            }

            var bookingResult = new BookingResult
            {
                Booking = booking,
                BookingId = Guid.NewGuid()
            };

            bookingRepository.CreateBooking(bookingResult);

            return bookingResult;
        }

        private bool CanCreateBooking(TimeOnly potentialBookingTime)
        {
            var existingBookingTimes = bookingRepository.GetBookings().Select(x => x.Booking.BookingTime).ToList();

            var allBookingTimes = existingBookingTimes
                .Append(potentialBookingTime)
                .OrderBy(t => t)
                .ToList();

            foreach (var bookingTime in allBookingTimes)
            {
                var start = bookingTime;
                var end = bookingTime.AddHours(1);

                var countOfBookingTimesWithinOneHour = allBookingTimes.Count(b => b >= start && b < end);

                if (countOfBookingTimesWithinOneHour > MaxSimultaneousBookings)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
