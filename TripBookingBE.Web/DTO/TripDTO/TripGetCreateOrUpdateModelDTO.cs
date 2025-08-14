using System.Net;
using TripBookingBE.Models;

namespace TripBookingBE.DTO.TripDTO;

public class TripGetCreateOrUpdateDTO
{
    public Trip? Trip { get; set; } = null;
    public HttpStatusCode RespCode { get; set; } = HttpStatusCode.OK;
    public string Message { get; set; } = string.Empty;
}