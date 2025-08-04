

using System.Net;
using TripBookingBE.Models;

namespace TripBookingBE.DTO.TripDTO;

public class TripGetTripsDTO
{
    public List<Trip> Trips { get; set; } = null;
    public HttpStatusCode RespCode = HttpStatusCode.OK;

    public string Message = string.Empty;
}