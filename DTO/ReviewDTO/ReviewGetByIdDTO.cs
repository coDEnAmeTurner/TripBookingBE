using System.Net;
using TripBookingBE.Models;

namespace TripBookingBE.DTO.ReviewDTO;

public class ReviewGetByIdDTO
{
    public CustomerReviewTrip Review = null;

    public HttpStatusCode RespCode = HttpStatusCode.OK;

    public string Message = string.Empty;
}