using Moq;
using SettlementBookingSystem.CustomExceptions;
using SettlementBookingSystem.Models;
using SettlementBookingSystem.Repositories;
using SettlementBookingSystem.Services;

namespace SettlementBookingSystemTests
{
    public class BookingServiceTests
    {
        private readonly Mock<IBookingRepository> _mockRepository;
        private readonly BookingService _bookingService;

        public BookingServiceTests()
        {
            _mockRepository = new Mock<IBookingRepository>();
            _bookingService = new BookingService(_mockRepository.Object);
        }

        [Fact]
        public void CreateBooking_WithValidBooking_ReturnsBookingResult()
        {
            // Arrange
            var booking = new Booking
            {
                BookingTime = new TimeOnly(10, 0),
                Name = "John Doe"
            };

            _mockRepository.Setup(r => r.GetBookings())
                .Returns(new List<BookingResult>().AsReadOnly());

            // Act
            var result = _bookingService.CreateBooking(booking);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(booking, result.Booking);
            Assert.NotEqual(Guid.Empty, result.BookingId);
            _mockRepository.Verify(r => r.CreateBooking(It.IsAny<BookingResult>()), Times.Once);
        }

        [Fact]
        public void CreateBooking_WithFourExactSameTimeBookings_ThrowsBookingConflictException()
        {
            // Arrange
            var booking = new Booking
            {
                BookingTime = new TimeOnly(10, 0),
                Name = "John Doe"
            };

            // Create 4 existing bookings at exactly 10:00
            var existingBookings = new List<BookingResult>
            {
                CreateBookingResult(new TimeOnly(10, 0), "Person 1"),
                CreateBookingResult(new TimeOnly(10, 0), "Person 2"),
                CreateBookingResult(new TimeOnly(10, 0), "Person 3"),
                CreateBookingResult(new TimeOnly(10, 0), "Person 4")
            }.AsReadOnly();

            _mockRepository.Setup(r => r.GetBookings())
                .Returns(existingBookings);

            // Act & Assert
            var exception = Assert.Throws<BookingConflictException>(() => _bookingService.CreateBooking(booking));
            Assert.Contains("Cannot create booking due to maximum (4) simultaneous bookings reached", exception.Message);
        }

        [Fact]
        public void CreateBooking_WithFourBookingsThatDontAllOverlapInSameHour_Success()
        {
            // Arrange - trying to book at 10:30
            var booking = new Booking
            {
                BookingTime = new TimeOnly(10, 30),
                Name = "John Doe"
            };

            // Create 4 existing bookings that don't all overlap in any single hour window
            var existingBookings = new List<BookingResult>
            {
                CreateBookingResult(new TimeOnly(9, 30), "Person 1"),    // 9:30-10:30 (only overlaps with 10:30 for a moment)
                CreateBookingResult(new TimeOnly(10, 0), "Person 2"),    // 10:00-11:00 (overlaps 10:30-11:00)
                CreateBookingResult(new TimeOnly(11, 0), "Person 3"),    // 11:00-12:00 (overlaps 11:00-11:30)
                CreateBookingResult(new TimeOnly(11, 30), "Person 4")    // 11:30-12:30 (no overlap with 10:30-11:30)
            }.AsReadOnly();

            _mockRepository.Setup(r => r.GetBookings())
                .Returns(existingBookings);

            // Act
            var result = _bookingService.CreateBooking(booking);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(booking, result.Booking);
            _mockRepository.Verify(r => r.CreateBooking(It.IsAny<BookingResult>()), Times.Once);
        }

        [Fact]
        public void CreateBooking_WithBookingsOutsideOneHourWindow_Success()
        {
            // Arrange
            var booking = new Booking
            {
                BookingTime = new TimeOnly(12, 0), // 12:00
                Name = "John Doe"
            };

            // Create bookings outside the 1-hour window
            var existingBookings = new List<BookingResult>
            {
                CreateBookingResult(new TimeOnly(10, 0), "Person 1"),   // 10:00-11:00 (no overlap with 12:00-13:00)
                CreateBookingResult(new TimeOnly(10, 30), "Person 2"),  // 10:30-11:30 (no overlap)
                CreateBookingResult(new TimeOnly(13, 30), "Person 3"),  // 13:30-14:30 (no overlap)
                CreateBookingResult(new TimeOnly(14, 0), "Person 4")    // 14:00-15:00 (no overlap)
            }.AsReadOnly();

            _mockRepository.Setup(r => r.GetBookings())
                .Returns(existingBookings);

            // Act
            var result = _bookingService.CreateBooking(booking);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(booking, result.Booking);
            _mockRepository.Verify(r => r.CreateBooking(It.IsAny<BookingResult>()), Times.Once);
        }

        [Fact]
        public void CreateBooking_WithThreeOverlappingBookings_Success()
        {
            // Arrange - trying to book at 10:30
            var booking = new Booking
            {
                BookingTime = new TimeOnly(10, 30),
                Name = "John Doe"
            };

            // Create only 3 bookings that would overlap with new booking
            var existingBookings = new List<BookingResult>
            {
                CreateBookingResult(new TimeOnly(10, 0), "Person 1"),   // 10:00-11:00 (overlaps)
                CreateBookingResult(new TimeOnly(10, 45), "Person 2"),  // 10:45-11:45 (overlaps)
                CreateBookingResult(new TimeOnly(11, 0), "Person 3")    // 11:00-12:00 (overlaps)
            }.AsReadOnly();

            _mockRepository.Setup(r => r.GetBookings())
                .Returns(existingBookings);

            // Act
            var result = _bookingService.CreateBooking(booking);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(booking, result.Booking);
            _mockRepository.Verify(r => r.CreateBooking(It.IsAny<BookingResult>()), Times.Once);
        }

        [Fact]
        public void CreateBooking_WithEmptyRepository_Success()
        {
            // Arrange
            var booking = new Booking
            {
                BookingTime = new TimeOnly(10, 0),
                Name = "John Doe"
            };

            _mockRepository.Setup(r => r.GetBookings())
                .Returns(new List<BookingResult>().AsReadOnly());

            // Act
            var result = _bookingService.CreateBooking(booking);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(booking, result.Booking);
            Assert.NotEqual(Guid.Empty, result.BookingId);
        }

        [Fact]
        public void CreateBooking_WithBookingAtExactBoundary_HandledCorrectly()
        {
            // Arrange
            var booking = new Booking
            {
                BookingTime = new TimeOnly(11, 0), // Exactly at boundary
                Name = "John Doe"
            };

            // Existing booking from 10:00-11:00 (ends exactly when new one starts)
            var existingBookings = new List<BookingResult>
            {
                CreateBookingResult(new TimeOnly(10, 0), "Person 1") // 10:00-11:00
            }.AsReadOnly();

            _mockRepository.Setup(r => r.GetBookings())
                .Returns(existingBookings);

            // Act
            var result = _bookingService.CreateBooking(booking);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(booking, result.Booking);
        }

        [Fact]
        public void CreateBooking_UnderstandTheAlgorithm_FourBookingsAt10am_ShouldBlock10_30()
        {
            // This test is to understand how the algorithm works
            // If we have 4 bookings at 10:00, and try to book at 10:30
            // The algorithm checks each time slot:
            // For 10:00 slot: counts bookings from 10:00-11:00 (should be 4 existing + 1 new = 5, which exceeds max)

            // Arrange
            var booking = new Booking
            {
                BookingTime = new TimeOnly(10, 30),
                Name = "John Doe"
            };

            var existingBookings = new List<BookingResult>
            {
                CreateBookingResult(new TimeOnly(10, 0), "Person 1"),
                CreateBookingResult(new TimeOnly(10, 0), "Person 2"),
                CreateBookingResult(new TimeOnly(10, 0), "Person 3"),
                CreateBookingResult(new TimeOnly(10, 0), "Person 4")
            }.AsReadOnly();

            _mockRepository.Setup(r => r.GetBookings())
                .Returns(existingBookings);

            // Act & Assert
            var exception = Assert.Throws<BookingConflictException>(() => _bookingService.CreateBooking(booking));
            Assert.Contains("Cannot create booking due to maximum (4) simultaneous bookings reached", exception.Message);
        }

        [Fact]
        public void CreateBooking_FiveBookingsInSameHourWindow_ThrowsBookingConflictException()
        {
            // Testing the edge case where we have exactly 5 bookings that all fall within the same hour window
            var booking = new Booking
            {
                BookingTime = new TimeOnly(10, 20),
                Name = "John Doe"
            };

            var existingBookings = new List<BookingResult>
            {
                CreateBookingResult(new TimeOnly(10, 0), "Person 1"),   // 10:00-11:00
                CreateBookingResult(new TimeOnly(10, 10), "Person 2"),  // 10:10-11:10
                CreateBookingResult(new TimeOnly(10, 30), "Person 3"),  // 10:30-11:30
                CreateBookingResult(new TimeOnly(10, 40), "Person 4")   // 10:40-11:40
            }.AsReadOnly();

            _mockRepository.Setup(r => r.GetBookings())
                .Returns(existingBookings);

            // Act & Assert - for the 10:00 time slot, there would be 5 bookings (10:00, 10:10, 10:20, 10:30, 10:40) all within 10:00-11:00
            var exception = Assert.Throws<BookingConflictException>(() => _bookingService.CreateBooking(booking));
            Assert.Contains("Cannot create booking due to maximum (4) simultaneous bookings reached", exception.Message);
        }

        private static BookingResult CreateBookingResult(TimeOnly bookingTime, string name)
        {
            return new BookingResult
            {
                Booking = new Booking
                {
                    BookingTime = bookingTime,
                    Name = name
                },
                BookingId = Guid.NewGuid()
            };
        }
    }
}