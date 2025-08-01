using System.Net;
using TripBookingBE.Models;

namespace TripBookingBE.DTO.BookingDTO;

public class BookingGetBookingsDTO
{
    public List<CustomerBookTrip> Bookings { get; set; } = null;

    public HttpStatusCode StatusCode {get;set;} = HttpStatusCode.OK;

    public string Message {get;set;}
}