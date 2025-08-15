using System.Net;

namespace TripBookingBE.DTO.TripDTO;

public class TripAssignDTO
{
    public HttpStatusCode RespCode { get; set; } = HttpStatusCode.OK;
    public string Message {get;set;} = string.Empty;
}