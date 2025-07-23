using TripBookingBE.DTO.CustomerBookTripDTO;

namespace TripBookingBE.Dal.DalInterfaces;

public interface ICustomerBookTripsDal
{
    public Task<CustomerBookTripDeleteByUserDTO> DeleteCustomerBookTripsByUser(long userId);
}