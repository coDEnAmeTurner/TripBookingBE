using TripBookingBE.DTO;
using TripBookingBE.DTO.BookingDTO;
using TripBookingBE.DTO.CustomerReviewTripDTO;
using TripBookingBE.Models;

namespace TripBookingBE.Services.ServiceInterfaces;

public interface ICustomerReviewTripsService
{
    Task<CustomerReviewTripDeleteByUserDTO> DeleteCustomerReviewTripsByUser(long userId);
}