using System.Net;
using TripBookingBE.Models;

namespace TripBookingBE.DTO.ReviewDTO;

public class ReviewDeleteByIdDTO
{
    public CustomerReviewTrip Review = null;

    public HttpStatusCode StatusCode = HttpStatusCode.NoContent;

    public string Message = string.Empty;
}