using System.Net;
using TripBookingBE.Models;

namespace TripBookingBE.DTO.BookingDTO;

public class BookingGetCreateOrUpdateDTO
{
    public CustomerBookTrip CustomerBookTrip = null;

    public HttpStatusCode StatusCode = HttpStatusCode.OK;

    public string Message = string.Empty;
}