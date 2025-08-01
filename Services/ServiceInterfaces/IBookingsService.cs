using TripBookingBE.DTO;
using TripBookingBE.DTO.BookingDTO;
using TripBookingBE.Models;

namespace TripBookingBE.Services.ServiceInterfaces;

public interface IBookingsService
{
    Task<BookingDeleteByUserDTO> DeleteBookingsByUser(long userId);
    Task<BookingGetIdByCustomerIdAndTripIdDTO> GetIdByCustIdAndTripId(long? custId, long? tripId);
    Task<BookingGetBookingsDTO> GetBookings(string? customerName, string? registrationNumber, DateTime? departureTime, string? routeDescription);
}