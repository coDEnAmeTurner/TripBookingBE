using TripBookingBE.DTO;
using TripBookingBE.DTO.CustomerBookTripDTO;
using TripBookingBE.Models;

namespace TripBookingBE.Services.ServiceInterfaces;

public interface ICustomerBookTripsService
{
    Task<CustomerBookTripDeleteByUserDTO> DeleteCustomerBookTripsByUser(long userId);
}