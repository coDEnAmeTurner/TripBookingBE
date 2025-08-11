namespace TripBookingBE.RestRequests.Ticket;

public class TicketCreateRequest
{
    public long? CustomerId {get;set;}
    public long? TripId {get;set;}
    public decimal? Price {get;set;}
    public long? GeneralParamId {get;set;}
}