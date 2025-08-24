using TripBookingBE.Commons.DTO.TicketDTO;
using TripBookingBE.DTO.BookingDTO;
using TripBookingBE.Models;

namespace TripBookingBE.Dal.DalInterfaces;

public interface IBookingsDal
{
    public Task<BookingDeleteByUserDTO> DeleteBookingsByUser(long userId);
    public Task<BookingGetByIdDTO> GetBookingById(long id);
    public Task<BookingGetIdByCustomerIdAndTripIdDTO> GetIdByCustIdAndTripId(long? custId, long? tripId);
    public Task<BookingGetBookingByCustomerIdAndTripIdDTO> GetBookingByCustIdAndTripId(long? custId, long? tripId);

    public Task<BookingDeleteByIdDTO> DeleteBooking(CustomerBookTrip booking);

    Task<BookingGetBookingsDTO> GetBookings(string? customerName, string? registrationNumber, DateTime? departureTime, string? routeDescription);

    public Task<BookingCreateOrUpdateDTO> Create(Models.CustomerBookTrip booking);

    public Task<BookingCreateOrUpdateDTO> Update(Models.CustomerBookTrip booking);

}