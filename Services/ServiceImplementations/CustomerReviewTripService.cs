using TripBookingBE.Dal.DalInterfaces;
using TripBookingBE.DTO.CustomerReviewTripDTO;
using TripBookingBE.Services.ServiceInterfaces;

public class CustomerReviewTripsService : ICustomerReviewTripsService
{
    private readonly ICustomerReviewTripsDal reviewDAL;

    public CustomerReviewTripsService(ICustomerReviewTripsDal reviewDAL)
    {
        this.reviewDAL = reviewDAL;
    }

    public async Task<CustomerReviewTripDeleteByUserDTO> DeleteCustomerReviewTripsByUser(long userId)
    {
        return await reviewDAL.DeleteCustomerReviewTripsByUser(userId);
    }
}