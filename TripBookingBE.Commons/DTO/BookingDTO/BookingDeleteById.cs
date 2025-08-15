using System.Net;
using TripBookingBE.Models;

namespace TripBookingBE.DTO.BookingDTO;

public class BookingDeleteByIdDTO
{
    public CustomerBookTrip CustomerBookTrip = null;

    public HttpStatusCode RespCode = HttpStatusCode.NoContent;

    public string Message = string.Empty;
}