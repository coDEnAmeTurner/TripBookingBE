

using System.Net;
using TripBookingBE.Models;

namespace TripBookingBE.DTO.TicketDTO;

public class TicketGetTicketsDTO
{
    public List<Ticket> Tickets { get; set; } = null;
    public HttpStatusCode StatusCode = HttpStatusCode.OK;

    public string Message = string.Empty;
}