using System.Net;
using TripBookingBE.Models;

namespace TripBookingBE.DTO.CustomerBookTripDTO;

public class CustomerBookTripDeleteByUserDTO
{
    public IQueryable<CustomerBookTrip> CustomerBookTrips = null;

    public HttpStatusCode StatusCode = HttpStatusCode.NoContent;

    public string Message = string.Empty;
}