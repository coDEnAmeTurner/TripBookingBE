using System.Net;
using System.Transactions;
using Microsoft.Extensions.Logging.Abstractions;
using TripBookingBE.Dal.DalInterfaces;
using TripBookingBE.DTO.RouteDTO;
using TripBookingBE.DTO.TripDTO;
using TripBookingBE.Services.ServiceInterfaces;

namespace TripBookingBE.Services.ServiceImplementations;

public class TripService : ITripService
{
    private readonly ITripDAL tripDAL;

    public TripService(ITripDAL tripDAL)
    {
        this.tripDAL = tripDAL;
    }

    public async Task<TripGetTripsDTO> GetTrips(int? placeCount, int? routeId, int? driverId, string? registrationNumber, DateTime? departureTime)
    {
        var dto = await tripDAL.GetTrips( placeCount,  routeId,  driverId,  registrationNumber, departureTime);
        return dto;
    }
}