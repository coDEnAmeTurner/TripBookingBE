using System.Net;
using TripBookingBE.Models;

namespace TripBookingBE.DTO.TripDTO;

public class TripGetByIdDTO
{
    public Models.Trip? Trip { get; set; } = null;
    public HttpStatusCode StatusCode = HttpStatusCode.OK;
    public string Message = string.Empty;
}