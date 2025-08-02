using System.Globalization;
using TripBookingBE.Dal.DalImplementations;
using TripBookingBE.Dal.DalInterfaces;
using TripBookingBE.DTO.BookingDTO;
using TripBookingBE.Models;
using TripBookingBE.Services.ServiceInterfaces;

public class BookingsService : IBookingsService
{
    private readonly IBookingsDal bookingDAL;
    private readonly IUsersDal usersDAL;
    private readonly ITripDAL tripDAL;

    public BookingsService(IBookingsDal bookingDAL, IUsersDal usersDAL, ITripDAL tripDAL)
    {
        this.bookingDAL = bookingDAL;
        this.usersDAL = usersDAL;
        this.tripDAL = tripDAL;
    }

    public async Task<BookingCreateOrUpdateDTO> CreateOrUpdate(CustomerBookTrip booking)
    {
        BookingCreateOrUpdateDTO dto = new();

        if (booking.Id == 0)
        {
            var already_exist = await bookingDAL.GetIdByCustIdAndTripId(booking.CustomerId, booking.TripId);
            if (already_exist != null && already_exist.Ids != null && already_exist.Ids.Count > 0)
            {
                var customer = await usersDAL.GetUserById(booking.CustomerId);
                var trip = await tripDAL.GetTripById(booking.TripId);

                dto.CustomerBookTrip = booking;
                dto.StatusCode = System.Net.HttpStatusCode.Conflict;
                dto.Message = $"Customer {customer.User.Name} - Phone: {customer.User.Phone} already booked Trip {trip.Trip.Route.RouteDescription} - Departure Time: {trip.Trip.DepartureTime?.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)} - Regis Number: {trip.Trip.RegistrationNumber}";
                return dto;
            }

            dto = await bookingDAL.Create(booking);
        }
        else
        {
            //check for password, with jwt configured

            dto = await bookingDAL.Update(booking);
        }
        return dto;
    }

    public async Task<BookingDeleteByIdDTO> DeleteBooking(long id)
    {
        var booking  = await bookingDAL.GetBookingById(id);
        if (booking == null)
        {
            BookingDeleteByIdDTO dto = new();
            dto.StatusCode = System.Net.HttpStatusCode.NotFound;
            dto.Message = "The booking is not found!";
            return dto;
        }

        return await bookingDAL.DeleteBooking(booking.CustomerBookTrip);
    }

    public async Task<BookingDeleteByUserDTO> DeleteBookingsByUser(long userId)
    {

        return await bookingDAL.DeleteBookingsByUser(userId);
    }

    public async Task<BookingGetByIdDTO> GetBookingById(long id)
    {
        return await bookingDAL.GetBookingById(id);
    }

    public async Task<BookingGetBookingsDTO> GetBookings(string? customerName, string? registrationNumber, DateTime? departureTime, string? routeDescription)
    {
        return await bookingDAL.GetBookings(customerName, registrationNumber, departureTime, routeDescription);

    }

    public async Task<BookingGetCreateOrUpdateDTO> GetCreateOrUpdateModel(long? id)
    {
        BookingGetCreateOrUpdateDTO dto = new();

        if (id == null)
        {
            dto.CustomerBookTrip = new CustomerBookTrip();
        }
        else
        {
            var dtobyid = await bookingDAL.GetBookingById(id.GetValueOrDefault());
            dto.StatusCode = dtobyid.StatusCode;
            dto.Message = dtobyid.Message;
            dto.CustomerBookTrip = dtobyid.CustomerBookTrip;
            
        }

        return dto;
    }

    public async Task<BookingGetIdByCustomerIdAndTripIdDTO> GetIdByCustIdAndTripId(long? custId, long? tripId)
    {
        return await bookingDAL.GetIdByCustIdAndTripId(custId, tripId);
    }
}