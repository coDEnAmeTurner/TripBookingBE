using System.Net;
using TripBookingBE.Models;

namespace TripBookingBE.DTO.TripDTO;

public class TripCreateOrUpdateDTO
{
    public Models.Trip Trip { get; set; } = null;
    public HttpStatusCode RespCode { get; set; } = HttpStatusCode.Created;
    public string Message { get; set; } = string.Empty;
}