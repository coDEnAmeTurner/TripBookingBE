namespace TripBookingBE.RestRequests.Route;

public class RouteListRequest
{
    public string? Description { get; set; }
    public string? DateCreated { get; set; }
    public int? PageNumber { get; set; }
}