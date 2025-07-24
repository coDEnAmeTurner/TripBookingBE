using System.Net;
using TripBookingBE.Models;

namespace TripBookingBE.DTO.RouteDTO;

public class RouteCreateOrUpdateDTO
{
    public Models.Route Route { get; set; } = null;
    public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.Created;
    public string Message { get; set; } = string.Empty;
}