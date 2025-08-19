using System.Data.Common;
using System.Globalization;
using System.Net;
using Microsoft.EntityFrameworkCore;
using TripBookingBE.Dal.DalInterfaces;
using TripBookingBE.Data;
using TripBookingBE.DTO.BookingDTO;
using TripBookingBE.Models;

public class BookingsDal : IBookingsDal
{
    private readonly TripBookingContext context;
    public BookingsDal(TripBookingContext context)
    {
        this.context = context;
    }               

    public async Task<BookingCreateOrUpdateDTO> Create(CustomerBookTrip booking)
    {
        BookingCreateOrUpdateDTO dto = new();

        try
        {
            context.Add(booking);
            await context.SaveChangesAsync();

        }
        catch (Exception ex)
        {
            dto.RespCode = System.Net.HttpStatusCode.BadRequest;
            dto.Message = $"{ex.Message}\n{ex.InnerException?.Message}";
        }
        finally     
        {
            dto.CustomerBookTrip = booking;

        }

        return dto;
    }

    public async Task<BookingDeleteByIdDTO> DeleteBooking(CustomerBookTrip booking)
    {
        BookingDeleteByIdDTO dto = new();

        try
        {
            context.CustomerBookTrips.Remove(booking);
            await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            dto.RespCode = HttpStatusCode.InternalServerError;
            dto.Message = $"{ex.Message}\n{ex.InnerException?.Message}";
        }

        dto.CustomerBookTrip = booking;

        return dto;
    }

    public async Task<BookingDeleteByUserDTO> DeleteBookingsByUser(long userId)
    {
        BookingDeleteByUserDTO dto = new();

        try
        {
            var bookings = context.CustomerBookTrips.Where(x => x.CustomerId == userId);
            context.CustomerBookTrips.RemoveRange(bookings);
            await context.SaveChangesAsync();
            dto.CustomerBookTrips = bookings;
        }
        catch (Exception ex)
        {
            dto.RespCode = System.Net.HttpStatusCode.InternalServerError;
            dto.Message = ex.Message;
        }
        return dto;
    }

    public async Task<BookingGetByIdDTO> GetBookingById(long id)
    {
        BookingGetByIdDTO dto = new();
        try
        {
            var cbt = await context.CustomerBookTrips
            .Include(e => e.Customer)
            .Include(e => e.Trip)
                .ThenInclude(e => e.Route)
            .FirstOrDefaultAsync(x=>x.Id == id);
            if (cbt == null)
            {
                dto.RespCode = System.Net.HttpStatusCode.InternalServerError;
                dto.Message = $"User with Id {id} not found!";
            }
            dto.CustomerBookTrip = cbt;
        }
        catch (Exception ex)
        {
            dto.RespCode = System.Net.HttpStatusCode.InternalServerError;
            dto.Message = ex.Message;
        }
        return dto;
    }

    public async Task<BookingGetBookingsDTO> GetBookings(string? customerName, string? registrationNumber, DateTime? departureTime, string? routeDescription)
    {
        BookingGetBookingsDTO dto = new();
        try
        {
            var bookings = from booking in context.CustomerBookTrips
                           where (customerName == null || booking.Customer.Name.Contains(customerName))
                            && (registrationNumber == null || booking.Trip.RegistrationNumber.Contains(registrationNumber))
                            && (departureTime == null || (booking.Trip.DepartureTime.HasValue && booking.Trip.DepartureTime == departureTime))
                            && (routeDescription == null || (booking.Trip.Route != null && booking.Trip.Route.RouteDescription != null && booking.Trip.Route.RouteDescription.Contains(routeDescription)))
                           orderby booking.DateCreated descending
                           select booking;
            var list = await bookings.Include(b=>b.Trip).ThenInclude(t=>t.Route).Include(b=>b.Customer).ToListAsync();
            
            dto.Bookings = list;

        }
        catch (Exception ex)
        {
            dto.RespCode = HttpStatusCode.InternalServerError;
            dto.Message = $"{ex.Message}\n{ex.InnerException?.Message}";
        }

        return dto;
    }

    public async Task<BookingGetIdByCustomerIdAndTripIdDTO> GetIdByCustIdAndTripId(long? custId, long? tripId)
    {
        BookingGetIdByCustomerIdAndTripIdDTO dto = new();
        try
        {
            var ids = from cbt in context.CustomerBookTrips
                      where (tripId == null || tripId == cbt.TripId)
                      && (custId == null || custId == cbt.CustomerId)
                      select cbt.Id;
            var result = await ids.ToListAsync();

            dto.Ids = result;

        }
        catch (Exception ex)
        {
            dto.RespCode = System.Net.HttpStatusCode.InternalServerError;
            dto.Message = $"{ex.Message}\n{ex.InnerException?.Message}";
        }

        return dto;
    }

    public async Task<BookingCreateOrUpdateDTO> Update(CustomerBookTrip booking)
    {
        BookingCreateOrUpdateDTO dto = new();
        try
        {
            var currentState = context.Entry(booking).State;
            context.Entry(booking).State = EntityState.Modified;
            context.Update(booking);
            await context.SaveChangesAsync();

        }
        // catch (DbUpdateConcurrencyException ex)
        // {
        //     dto.RespCode = HttpStatusCode.Conflict;

        //     var exceptionEntry = ex.Entries.Single();
        //     var clientValues = (CustomerBookTrip)exceptionEntry.Entity;
        //     var databaseEntry = exceptionEntry.GetDatabaseValues();
        //     if (databaseEntry == null)
        //     {
        //         dto.Message =
        //             "Unable to save changes. The User was deleted by another user.";
        //     }
        //     else
        //     {
        //         var databaseValues = (CustomerBookTrip)databaseEntry.ToObject();

        //         if (databaseValues.CustomerId != clientValues.CustomerId)
        //         {
        //             dto.Message = $"Customer - Current value: {databaseValues.Customer.Name} - Phone: {databaseValues.Customer.Phone} - Email: {databaseValues.Customer.Email}";
        //         }
        //         if (databaseValues.TripId != clientValues.TripId) 
        //         {
        //             dto.Message = $"Trip - Current value: {databaseValues.Trip.Route?.RouteDescription} - Departure Time: {databaseValues.Trip.DepartureTime?.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)} - Registration Number: {databaseValues.Trip.RegistrationNumber}";
        //         }

        //         dto.Message += "\nThe record you attempted to edit "
        //                 + "was modified by another user after you got the original value. The "
        //                 + "edit operation was canceled and the current values in the database "
        //                 + "have been displayed. If you still want to edit this record, click "
        //                 + "the Save button again. Otherwise click the Back to List hyperlink.";
        //         booking.RowVersion = (byte[])databaseValues.RowVersion;
        //     }
        // }
        catch (Exception ex)
        {
            dto.Message = ex.Message;
            dto.RespCode = System.Net.HttpStatusCode.InternalServerError;
        }
        finally
        {
            dto.CustomerBookTrip = booking;

        }

        return dto;
    }
}