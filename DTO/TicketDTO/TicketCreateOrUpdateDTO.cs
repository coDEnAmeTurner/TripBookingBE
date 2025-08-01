using System.Net;
using TripBookingBE.Models;

namespace TripBookingBE.DTO.TicketDTO;

public class TicketCreateOrUpdateDTO
{
    public Models.Ticket Ticket { get; set; } = null;
    public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.Created;
    public string Message { get; set; } = string.Empty;
}