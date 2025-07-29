using TripBookingBE.DTO.TripDTO;
using TripBookingBE.Models;

namespace TripBookingBE.Services.ServiceInterfaces;

public interface ITripService
{
    Task<TripGetTripsDTO> GetTrips(int? placeCount, int? routeId, int? driverId, string? registrationNumber, DateTime? departureTime);

    Task<TripGetCreateOrUpdateDTO> GetCreateOrUpdateModel(long? id);

    Task<TripCreateOrUpdateDTO> CreateOrUpdate(Trip trip);

    Task<TripGetByIdDTO> GetTripById(long id);

    Task<TripDeleteDTO> DeleteTrip(long id);

}