using System.Net;
using TripBookingBE.Models;

namespace TripBookingBE.DTO.TicketDTO;

public class TicketDeleteDTO
{
    public Models.Ticket Ticket { get; set; } = null;
    public HttpStatusCode StatusCode = HttpStatusCode.NoContent;
    public string Message = string.Empty;
}