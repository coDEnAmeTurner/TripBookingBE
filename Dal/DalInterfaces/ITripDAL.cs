using NuGet.Common;
using TripBookingBE.DTO.RouteDTO;
using TripBookingBE.DTO.TripDTO;

namespace  TripBookingBE.Dal.DalInterfaces;

public interface ITripDAL
{
    public Task<TripGetTripsDTO> GetTrips(int? placeCount, int? routeId, int? driverId, string? registrationNumber, DateTime? departureTime);

    public Task<TripGetByIdDTO> GetTripById(long id);

    public Task<TripCreateOrUpdateDTO> Create(Models.Trip trip);

    public Task<TripCreateOrUpdateDTO> Update(Models.Trip trip);

    public Task<TripDeleteDTO> DeleteTrip(long id);

    public Task<TripCheckSeatDTO> CheckSeat(long id, int placeNumber);
}