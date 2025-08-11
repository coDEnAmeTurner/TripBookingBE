namespace TripBookingBE.RestRequests.Trip;

public class TripListRequest
{
    public int? PlaceCount {get;set;}
    public int? RouteId {get;set;}
    public int? DriverId {get;set;}
    public string? RegistrationNumber {get;set;}
    public string DepartureTime {get;set;}
    public int? PageNumber {get;set;}
}