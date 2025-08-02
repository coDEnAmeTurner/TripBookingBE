using System.Net;
using TripBookingBE.Models;

namespace TripBookingBE.DTO.ReviewDTO;

public class ReviewGetReviewsDTO
{
    public List<CustomerReviewTrip> Reviews = null;

    public HttpStatusCode StatusCode = HttpStatusCode.OK;

    public string Message = string.Empty;
}