namespace TripBookingBE.RestRequests.Ticket;

public class TicketListRequest
{
    public long? CustomerId { get; set; }
    public long? TripId { get; set; }
    public decimal? FromPrice { get; set; }
    public decimal? ToPrice { get; set; }
    public string? SellerCode { get; set; }
    public string? DepartureTime { get; set; }
    public long? GeneralParamId { get; set; }
    public int? PageNumber { get; set; }
}