using System.Net;
using TripBookingBE.Models;

namespace TripBookingBE.DTO.CustomerBookTripDTO;

public class CustomerBookTripGetIdByCustomerIdAndTripIdDTO
{
    public List<long> Ids = null;

    public HttpStatusCode StatusCode = HttpStatusCode.OK;

    public string Message = string.Empty;
}