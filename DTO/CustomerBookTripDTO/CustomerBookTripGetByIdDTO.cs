using System.Net;
using TripBookingBE.Models;

namespace TripBookingBE.DTO.CustomerBookTripDTO;

public class CustomerBookTripGetByIdDTO
{
    public CustomerBookTrip CustomerBookTrip = null;

    public HttpStatusCode StatusCode = HttpStatusCode.NoContent;

    public string Message = string.Empty;
}