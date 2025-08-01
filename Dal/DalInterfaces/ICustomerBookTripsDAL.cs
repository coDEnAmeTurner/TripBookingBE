using TripBookingBE.DTO.CustomerBookTripDTO;

namespace TripBookingBE.Dal.DalInterfaces;

public interface ICustomerBookTripsDal
{
    public Task<CustomerBookTripDeleteByUserDTO> DeleteCustomerBookTripsByUser(long userId);
    public Task<CustomerBookTripGetByIdDTO> GetCustomerBookTripById(long id);

    public Task<CustomerBookTripGetIdByCustomerIdAndTripIdDTO> GetIdByCustIdAndTripId(long? custId, long? tripId);

}