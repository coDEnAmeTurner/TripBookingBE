using System.Net;
using TripBookingBE.Models;

namespace TripBookingBE.DTO.ReviewDTO;

public class ReviewDeleteByUserDTO
{
    public IQueryable<CustomerReviewTrip> CustomerReviewTrips = null;

    public HttpStatusCode RespCode = HttpStatusCode.NoContent;

    public string Message = string.Empty;
}