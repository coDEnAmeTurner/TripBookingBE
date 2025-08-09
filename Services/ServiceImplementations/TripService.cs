using System.Net;
using System.Transactions;
using TripBookingBE.Dal.DalInterfaces;
using TripBookingBE.DTO.TripDTO;
using TripBookingBE.Models;
using TripBookingBE.Services.ServiceInterfaces;

namespace TripBookingBE.Services.ServiceImplementations;

public class TripService : ITripService
{
    private readonly ITripDAL tripDAL;

    private readonly IUsersDal usersDAL;
    private readonly IRouteDAL routeDAL;

    private readonly IBookingsDal bookingDAL;

    public TripService(ITripDAL tripDAL, IUsersDal usersDAL, IRouteDAL routeDAL, IBookingsDal bookingDAL)
    {
        this.tripDAL = tripDAL;
        this.usersDAL = usersDAL;
        this.routeDAL = routeDAL;
        this.bookingDAL = bookingDAL;
    }

    public async Task<TripAssignDTO> AssignDriver(int tripId, int driverId)
    {
        var dto = new TripAssignDTO();
        var userdto = await usersDAL.GetUserById(driverId);

        if (userdto.RespCode != HttpStatusCode.OK)
        {
            dto.RespCode = HttpStatusCode.NotFound;
            dto.Message = $"Driver of id: {driverId} is not found!";
            return dto;
        }

        var tripdto = await tripDAL.GetTripById(tripId);
        var dbtrip = tripdto.Trip;
        dbtrip.Driver = userdto.User;
        var editdto = await tripDAL.Update(dbtrip);
           
        dto.RespCode = editdto.RespCode;
        dto.Message = editdto.Message;
        return dto;
    }

    public async Task<TripAssignDTO> AssignRoute(int tripId, int routeId)
    {
        var dto = new TripAssignDTO();
        var routedto = await routeDAL.GetRouteById(routeId);

        if (routedto.RespCode != HttpStatusCode.OK)
        {
            dto.RespCode = HttpStatusCode.NotFound;
            dto.Message = $"Route of id: {routeId} is not found!";
            return dto;
        }

        var tripdto = await tripDAL.GetTripById(tripId);
        var dbtrip = tripdto.Trip;
        dbtrip.Route = routedto.Route;
        var editdto = await tripDAL.Update(dbtrip);
           
        dto.RespCode = editdto.RespCode;
        dto.Message = editdto.Message;
        return dto;
    }

    public async Task<TripBookDTO> Book(long tripId, long userId, int placeNumber)
    {
        var dto = new TripBookDTO();
        var tripdto = await tripDAL.GetTripById(tripId);
        if (tripdto.RespCode != HttpStatusCode.OK)
        {
            dto.RespCode = tripdto.RespCode;
            dto.Message = tripdto.Message;
            return dto;
        }

        var userdto = await usersDAL.GetUserById(userId);
        if (userdto.RespCode != HttpStatusCode.OK)
        {
            dto.RespCode = userdto.RespCode;
            dto.Message = userdto.Message;
            return dto;
        }

        var booking = new CustomerBookTrip()
        {
            Trip = tripdto.Trip,
            Customer = userdto.User,
            PlaceNumber = placeNumber
        };
        var bookingdto = await bookingDAL.Create(booking);
        dto.RespCode = bookingdto.RespCode;
        dto.Message = bookingdto.Message;
        return dto;
    }

    public async Task<TripCreateOrUpdateDTO> CreateOrUpdate(Trip trip)
    {
        TripCreateOrUpdateDTO dto = new();

        if (trip.Id == 0)
        {

            dto = await tripDAL.Create(trip);
        }
        else
        {
            //check for password, with jwt configured
            dto = await tripDAL.Update(trip);
        }
        return dto;
    }

    public async Task<TripDeleteDTO> DeleteTrip(long id)
    {
        TripDeleteDTO dto = new();
        using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            try
            {

                var tripDTO = await tripDAL.DeleteTrip(id);
                if (tripDTO.RespCode != HttpStatusCode.NoContent)
                {
                    dto.RespCode = tripDTO.RespCode;
                    dto.Message += $"\n{tripDTO.Message}";
                }

                scope.Complete();
            }
            catch (Exception ex)
            {
                if (dto.RespCode == HttpStatusCode.NoContent)
                {
                    dto.RespCode = HttpStatusCode.InternalServerError;
                    dto.Message = ex.Message;
                }
            }

        }
        return dto;
    }

    public async Task<TripGetCreateOrUpdateDTO> GetCreateOrUpdateModel(long? id)
    {
        TripGetCreateOrUpdateDTO dto = new();
        if (id == null)
            dto.Trip = new Models.Trip();
        else
        {
            var dtoDAL = await tripDAL.GetTripById(id.GetValueOrDefault());
            dto.Trip = dtoDAL.Trip;
            dto.RespCode = dtoDAL.RespCode;
            dto.Message = dtoDAL.Message;
        }

        return dto;
    }

    public async Task<TripGetByIdDTO> GetTripById(long id)
    {
        var dto = await tripDAL.GetTripById(id);
        return dto;
    }

    public async Task<TripGetTripsDTO> GetTrips(int? placeCount, int? routeId, int? driverId, string? registrationNumber, DateTime? departureTime)
    {
        var dto = await tripDAL.GetTrips( placeCount,  routeId,  driverId,  registrationNumber, departureTime);
        return dto;
    }
}