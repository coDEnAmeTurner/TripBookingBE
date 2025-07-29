using Microsoft.EntityFrameworkCore.Metadata.Internal;
using TripBookingBE.DTO.RouteDTO;
using TripBookingBE.DTO.TripDTO;

namespace TripBookingBE.Services.ServiceInterfaces;

public interface ITripService
{
    public Task<TripGetTripsDTO> GetTrips(int? placeCount, int? routeId, int? driverId, string? registrationNumber, string departureTime, int? pageNumber);

}