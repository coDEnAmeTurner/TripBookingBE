using System.Net;

namespace TripBookingBE.DTO.RouteDTO;

public class RouteGetCreateOrUpdateModelDTO
{
    public Models.Route Route { get; set; } = null;
    public HttpStatusCode RespCode { get; set; } = HttpStatusCode.Created;
    public string Message { get; set; } = string.Empty;
}