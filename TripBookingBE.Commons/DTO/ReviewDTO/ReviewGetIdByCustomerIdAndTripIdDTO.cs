using System.Net;
using TripBookingBE.Models;

namespace TripBookingBE.DTO.ReviewDTO;

public class ReviewGetIdByCustomerIdAndTripIdDTO
{
    public List<long> Ids = null;

    public HttpStatusCode RespCode = HttpStatusCode.OK;

    public string Message = string.Empty;
}