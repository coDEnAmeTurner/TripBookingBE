using System.Net;
using TripBookingBE.Models;

namespace TripBookingBE.DTO.TripDTO;

public class TripDeleteDTO
{
    public Models.Trip Trip { get; set; } = null;
    public HttpStatusCode RespCode = HttpStatusCode.NoContent;
    public string Message = string.Empty;
}