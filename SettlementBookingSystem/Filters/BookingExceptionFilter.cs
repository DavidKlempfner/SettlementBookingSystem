using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SettlementBookingSystem.CustomExceptions;

namespace SettlementBookingSystem.Filters
{
    public class BookingExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if (context.Exception is BookingConflictException bookingConflictException)
            {
                context.Result = new ConflictObjectResult(new { error = bookingConflictException.Message });
            }
        }
    }
}
