
using System.Net;
using TripBookingBE.Models;

namespace TripBookingBE.DTO.RouteDTO;

public class RouteGetRoutesDTO
{
    public List<Models.Route> Routes { get; set; } = null;
    public HttpStatusCode StatusCode = HttpStatusCode.OK;

    public string Message = string.Empty;
}