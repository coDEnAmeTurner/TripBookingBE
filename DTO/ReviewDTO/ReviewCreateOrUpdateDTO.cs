using System.Net;
using TripBookingBE.Models;

namespace TripBookingBE.DTO.ReviewDTO;

public class ReviewCreateOrUpdateDTO
{
    public Models.CustomerReviewTrip Review { get; set; } = null;
    public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.Created;
    public string Message { get; set; } = string.Empty;
}