using TripBookingBE.Dal.DalInterfaces;
using TripBookingBE.DTO.BookingDTO;
using TripBookingBE.Models;
using TripBookingBE.Services.ServiceInterfaces;

public class BookingsService : IBookingsService
{
    private readonly IBookingsDal bookingDAL;

    public BookingsService(IBookingsDal bookingDAL)
    {
        this.bookingDAL = bookingDAL;
    }

    public async Task<BookingDeleteByUserDTO> DeleteBookingsByUser(long userId)
    {
        
        return await bookingDAL.DeleteBookingsByUser(userId);
    }

    public async Task<BookingGetBookingsDTO> GetBookings(string? customerName, string? registrationNumber, DateTime? departureTime, string? routeDescription)
    {
        return await bookingDAL.GetBookings(customerName, registrationNumber, departureTime, routeDescription);

    }

    public async Task<BookingGetIdByCustomerIdAndTripIdDTO> GetIdByCustIdAndTripId(long? custId, long? tripId)
    {
        return await bookingDAL.GetIdByCustIdAndTripId(custId, tripId);
    }
}