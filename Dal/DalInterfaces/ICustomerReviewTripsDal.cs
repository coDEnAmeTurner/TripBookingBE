using TripBookingBE.DTO.CustomerReviewTripDTO;

namespace TripBookingBE.Dal.DalInterfaces;

public interface ICustomerReviewTripsDal
{
    public Task<CustomerReviewTripDeleteByUserDTO> DeleteCustomerReviewTripsByUser(long userId);
}