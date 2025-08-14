namespace TripBookingBE.RestRequests.Trip;

public class TripCreateRequest
{
    public string? DepartureTime { get; set; }
    public int? PlaceCount {get;set;}
    public string? RegistrationNumber {get;set;}
    public int? DriverId {get;set;}
    public int? RouteId {get;set;}
}