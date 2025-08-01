using System.Data.Common;
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
            dto.StatusCode = HttpStatusCode.InternalServerError;
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
            dto.StatusCode = System.Net.HttpStatusCode.InternalServerError;
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
                dto.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                dto.Message = $"User with Id {id} not found!";
            }
            dto.CustomerBookTrip = cbt;
        }
        catch (Exception ex)
        {
            dto.StatusCode = System.Net.HttpStatusCode.InternalServerError;
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
            dto.StatusCode = HttpStatusCode.InternalServerError;
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
            dto.StatusCode = System.Net.HttpStatusCode.InternalServerError;
            dto.Message = $"{ex.Message}\n{ex.InnerException?.Message}";
        }

        return dto;
    }
}