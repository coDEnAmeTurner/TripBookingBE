using System.Net;
using TripBookingBE.Models;

namespace TripBookingBE.DTO.CustomerReviewTripDTO;

public class CustomerReviewTripDeleteByUserDTO
{
    public IQueryable<CustomerReviewTrip> CustomerReviewTrips = null;

    public HttpStatusCode StatusCode = HttpStatusCode.NoContent;

    public string Message = string.Empty;
}