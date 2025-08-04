using System.Net;
using TripBookingBE.Models;

namespace TripBookingBE.DTO.TicketDTO;

public class TicketGetCreateOrUpdateDTO
{
    public Models.Ticket Ticket { get; set; } = null;
    public HttpStatusCode RespCode { get; set; } = HttpStatusCode.OK;
    public string Message { get; set; } = string.Empty;
}