using System.Net;
using TripBookingBE.Models;

namespace TripBookingBE.DTO.ReviewDTO;

public class ReviewGetCreateOrUpdateDTO
{
    public CustomerReviewTrip Review = null;

    public HttpStatusCode StatusCode = HttpStatusCode.OK;

    public string Message = string.Empty;
}