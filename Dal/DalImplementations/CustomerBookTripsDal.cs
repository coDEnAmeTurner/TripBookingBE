using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using TripBookingBE.Dal.DalInterfaces;
using TripBookingBE.Data;
using TripBookingBE.DTO.CustomerBookTripDTO;

public class CustomerBookTripsDal : ICustomerBookTripsDal
{
    private readonly TripBookingContext context;
    public CustomerBookTripsDal(TripBookingContext context)
    {
        this.context = context;
    }

    public async Task<CustomerBookTripDeleteByUserDTO> DeleteCustomerBookTripsByUser(long userId)
    {
        CustomerBookTripDeleteByUserDTO dto = new();

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

    public async Task<CustomerBookTripGetByIdDTO> GetCustomerBookTripById(long id)
    {
        CustomerBookTripGetByIdDTO dto = new();
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
}