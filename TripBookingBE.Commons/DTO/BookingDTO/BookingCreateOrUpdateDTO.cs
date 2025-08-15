using System.Net;
using TripBookingBE.Models;

namespace TripBookingBE.DTO.BookingDTO;

public class BookingCreateOrUpdateDTO
{
    public Models.CustomerBookTrip CustomerBookTrip { get; set; } = null;
    public HttpStatusCode RespCode { get; set; } = HttpStatusCode.Created;
    public string Message { get; set; } = string.Empty;
}