using System.Net;
using Microsoft.EntityFrameworkCore;
using TripBookingBE.Dal.DalInterfaces;
using TripBookingBE.Data;
using TripBookingBE.DTO.TripDTO;
using TripBookingBE.Models;

namespace TripBookingBE.Dal.DalImplementations;

public class TripDAL : ITripDAL
{
    private readonly TripBookingContext context;

    public TripDAL(TripBookingContext context)
    {
        this.context = context;
    }

    public async Task<TripCreateOrUpdateDTO> Create(Trip trip)
    {
        TripCreateOrUpdateDTO dto = new();

        try
        {
            context.Add(trip);
            await context.SaveChangesAsync();

        }
        catch (Exception ex)
        {
            dto.StatusCode = System.Net.HttpStatusCode.InternalServerError;
            dto.Message = ex.Message;
        }
        finally
        {
            dto.Trip = trip;

        }

        return dto;
    }

    public async Task<TripDeleteDTO> DeleteTrip(long id)
    {
        TripDeleteDTO dto = new();

        var inst = await context.Trips.FindAsync(id);
        if (inst == null)
        {
            dto.StatusCode = System.Net.HttpStatusCode.NotFound;
            dto.Message += $"\nTrip with Id {id} not found!";
        }

        context.Trips.Remove(inst);
        await context.SaveChangesAsync();

        dto.Trip = inst;
        return dto;
    }

    public async Task<TripGetByIdDTO> GetTripById(long id)
    {
        TripGetByIdDTO dto = new();
        try
        {
            var trip = await context.Trips.FirstOrDefaultAsync(x => x.Id == id);
            if (trip == null)
            {
                dto.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                dto.Message = $"User with Id {id} not found!";
            }
            dto.Trip = trip;
        }
        catch (Exception ex)
        {
            dto.StatusCode = System.Net.HttpStatusCode.InternalServerError;
            dto.Message = ex.Message;
        }
        return dto;
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
            trips = from trip in trips
                    where departureTime == null ||
            (
                trip.DepartureTime != null &&
                trip.DepartureTime.Value.Year == departureTime.Value.Year
                && trip.DepartureTime.Value.Month == departureTime.Value.Month
                && trip.DepartureTime.Value.Date == departureTime.Value.Date
            )
                    select trip;

            var resulttrips = trips.OrderByDescending(u => u.Id).Include(e=>e.Driver).Include(e=>e.Route).ToList();
            resulttrips = (from trip in resulttrips where registrationNumber == null || (trip.RegistrationNumber != null && trip.RegistrationNumber.Contains(registrationNumber, StringComparison.OrdinalIgnoreCase)) select trip).ToList();

            dto.Trips = resulttrips;
        }
        catch (Exception ex)
        {
            dto.StatusCode = System.Net.HttpStatusCode.InternalServerError;
            dto.Message = ex.Message;
        }

        return dto;
    }

    public async Task<TripCreateOrUpdateDTO> Update(Trip trip)
    {
        TripCreateOrUpdateDTO dto = new();
        try
        {
            var currentState = context.Entry(trip).State;
            context.Entry(trip).State = EntityState.Modified;
            // context.Entry(user).Property("RowVersion").OriginalValue = user.RowVersion;
            context.Update(trip);
            await context.SaveChangesAsync();

        }
        catch (DbUpdateConcurrencyException ex)
        {
            dto.StatusCode = HttpStatusCode.Conflict;

            var exceptionEntry = ex.Entries.Single();
            var clientValues = (Models.Trip)exceptionEntry.Entity;
            var databaseEntry = exceptionEntry.GetDatabaseValues();
            if (databaseEntry == null)
            {
                dto.Message =
                    "Unable to save changes. The User was deleted by another user.";
            }
            else
            {
                var databaseValues = (Models.Trip)databaseEntry.ToObject();

                if (databaseValues.DepartureTime != clientValues.DepartureTime)
                {
                    dto.Message = $"Departure Time - Current value: {databaseValues.DepartureTime}";
                }

                if (databaseValues.PlaceCount != clientValues.PlaceCount)
                {
                    dto.Message = $"Place Count - Current value: {databaseValues.PlaceCount}";
                }

                if (databaseValues.RegistrationNumber != clientValues.RegistrationNumber)
                {
                    dto.Message = $"Registration Number - Current value: {databaseValues.RegistrationNumber}";
                }

                if (databaseValues.DriverId != clientValues.DriverId)
                {
                    dto.Message = $"Driver Id - Current value: {databaseValues.DriverId}";
                }

                if (databaseValues.RouteId != clientValues.RouteId)
                {
                    dto.Message = $"Route Id - Current value: {databaseValues.RouteId}";
                }

                dto.Message += "\nThe record you attempted to edit "
                        + "was modified by another user after you got the original value. The "
                        + "edit operation was canceled and the current values in the database "
                        + "have been displayed. If you still want to edit this record, click "
                        + "the Save button again. Otherwise click the Back to List hyperlink.";
                trip.RowVersion = (byte[])databaseValues.RowVersion;
            }
        }
        catch (Exception ex)
        {
            dto.Message = ex.Message;
            dto.StatusCode = System.Net.HttpStatusCode.InternalServerError;
        }
        finally
        {
            dto.Trip = trip;

        }

        return dto;
    }
}