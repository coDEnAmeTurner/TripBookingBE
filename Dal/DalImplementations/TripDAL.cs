using Microsoft.EntityFrameworkCore;
using TripBookingBE.Dal.DalInterfaces;
using TripBookingBE.Data;
using TripBookingBE.DTO.TripDTO;

namespace TripBookingBE.Dal.DalImplementations;

public class TripDAL : ITripDAL
{
    private readonly TripBookingContext context;

    public TripDAL(TripBookingContext context)
    {
        this.context = context;
    }

    public async Task<TripGetTripsDTO> GetTrips(int? placeCount, int? routeId, int? driverId, string? registrationNumber, DateTime? departureTime)
    {
        TripGetTripsDTO dto = new();
        try
        {
            var trips = from trip in context.Trips select trip;

            trips = from trip in trips where placeCount == null || trip.PlaceCount == placeCount.Value select trip;
            trips = from trip in trips where routeId == null || trip.RouteId == routeId.Value select trip;
            trips = from trip in trips where driverId == null || trip.DriverId == driverId.Value select trip;
            trips = from trip in trips where registrationNumber == null || (trip.RegistrationNumber != null && trip.RegistrationNumber.Contains(registrationNumber, StringComparison.OrdinalIgnoreCase)) select trip;
            trips = from trip in trips where departureTime == null ||
            (
                trip.DepartureTime != null &&
                trip.DepartureTime.Value.Year == DateTime.Now.Year
                && trip.DepartureTime.Value.Month == DateTime.Now.Month
                && trip.DepartureTime.Value.Day == DateTime.Now.Day
                && trip.DepartureTime.Value.Hour == DateTime.Now.Hour
                && trip.DepartureTime.Value.Minute == DateTime.Now.Minute
            )
             select trip;

            var resulttrips = trips.OrderByDescending(u => u.Id).ToList();

            dto.Trips = resulttrips;
        }
        catch (Exception ex)
        {
            dto.StatusCode = System.Net.HttpStatusCode.InternalServerError;
            dto.Message = ex.Message;
        }

        return dto;
    }
}