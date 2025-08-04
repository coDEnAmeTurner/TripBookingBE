using System.Net;
using TripBookingBE.Models;

namespace TripBookingBE.DTO.TicketDTO;

public class TicketGetByIdDTO
{
    public Models.Ticket Ticket { get; set; } = null;
    public HttpStatusCode RespCode = HttpStatusCode.OK;
    public string Message = string.Empty;
}