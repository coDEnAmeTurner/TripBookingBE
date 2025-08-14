using System.Net;

namespace TripBookingBE.DTO.TicketDTO;

public class TicketCheckOwnerDTO
{
    public bool IsOwner { get; set; } = false;
    public int RespCode { get; set; } = (int)HttpStatusCode.OK;
    public string? Message {get;set;} = string.Empty;
}