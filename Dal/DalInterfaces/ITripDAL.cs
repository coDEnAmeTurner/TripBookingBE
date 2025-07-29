using NuGet.Common;
using TripBookingBE.DTO.RouteDTO;
using TripBookingBE.DTO.TripDTO;

namespace  TripBookingBE.Dal.DalInterfaces;

public interface ITripDAL
{
    public Task<TripGetTripsDTO> GetTrips(int? placeCount, int? routeId, int? driverId, string? registrationNumber, DateTime? departureTime);

}