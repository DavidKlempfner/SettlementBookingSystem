using System.Text.Json.Serialization;

namespace SettlementBookingSystem.Models
{
    public class BookingResult
    {
        [JsonIgnore]
        public Booking Booking { get; set; }
        public Guid BookingId { get; set; }
    }
}
