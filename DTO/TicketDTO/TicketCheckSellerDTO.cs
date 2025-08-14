using System.Net;

namespace TripBookingBE.DTO.TicketDTO;

public class TicketCheckSellerDTO
{
    public bool IsSeller { get; set; } = false;
    public int RespCode { get; set; } = (int)HttpStatusCode.OK;
    public string? Message {get;set;} = string.Empty;
}