using TripBookingBE.DTO.TripDTO;
using TripBookingBE.Models;

namespace TripBookingBE.Services.ServiceInterfaces;

public interface ITripService
{
    Task<TripGetTripsDTO> GetTrips(int? placeCount, int? routeId, int? driverId, string? registrationNumber, DateTime? departureTime);

    Task<TripGetCreateOrUpdateDTO> GetCreateOrUpdateModel(long? id);

    Task<TripCreateOrUpdateDTO> CreateOrUpdate(Trip trip);

    Task<TripGetByIdDTO> GetTripById(long id);
    Task<TripBookDTO> Book(long tripId, long userId, int placeNumber);
    Task<TripReviewDTO> Review(long tripId, long userId, string? Content);

    Task<TripDeleteDTO> DeleteTrip(long id);

    Task<TripAssignDTO> AssignDriver(int tripId, int driverId);
    Task<TripAssignDTO> AssignRoute(int tripId, int routeId);

}