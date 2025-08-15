using System.Net;
using TripBookingBE.Models;

namespace TripBookingBE.DTO.RouteDTO;

public class RouteGetByIdDTO
{
    public Models.Route Route { get; set; } = null;
    public HttpStatusCode RespCode = HttpStatusCode.OK;
    public string Message = string.Empty;
}