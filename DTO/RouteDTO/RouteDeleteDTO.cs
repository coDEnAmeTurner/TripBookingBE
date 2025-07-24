using System.Net;
using TripBookingBE.Models;

namespace TripBookingBE.DTO.RouteDTO;

public class RouteDeleteDTO
{
    public Models.Route Route { get; set; } = null;
    public HttpStatusCode StatusCode = HttpStatusCode.NoContent;
    public string Message = string.Empty;
}