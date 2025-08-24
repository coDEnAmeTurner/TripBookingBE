using TripBookingBE.Models;

namespace TripBookingBE.Commons.DTO.TicketDTO;

public class BookingGetBookingByCustomerIdAndTripIdDTO
{
    public List<CustomerBookTrip> Bookings { get; set; } = new List<CustomerBookTrip>();

    public int RespCode { get; set; } = 200;
    public string? Message { get; set; } = "";
}