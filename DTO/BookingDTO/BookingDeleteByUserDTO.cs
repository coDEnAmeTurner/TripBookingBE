using System.Net;
using TripBookingBE.Models;

namespace TripBookingBE.DTO.BookingDTO;

public class BookingDeleteByUserDTO
{
    public IQueryable<CustomerBookTrip> CustomerBookTrips = null;

    public HttpStatusCode StatusCode = HttpStatusCode.NoContent;

    public string Message = string.Empty;
}