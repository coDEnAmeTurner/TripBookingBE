using System.Net;
using TripBookingBE.Models;

namespace TripBookingBE.DTO.BookingDTO;

public class BookingGetIdByCustomerIdAndTripIdDTO
{
    public List<long> Ids = null;

    public HttpStatusCode RespCode = HttpStatusCode.OK;

    public string Message = string.Empty;
}