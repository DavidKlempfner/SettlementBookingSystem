using System.ComponentModel.DataAnnotations;

namespace SettlementBookingSystem.Models
{
    public class Booking
    {
        [Required]
        [Range(typeof(TimeOnly), "09:00:00", "16:00:00", ErrorMessage = "Booking time must be between 09:00 and 16:00")]
        public TimeOnly BookingTime { get; set; }

        [Required]
        public required string Name { get; set; }
    }
}
