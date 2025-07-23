using TripBookingBE.Dal.DalInterfaces;
using TripBookingBE.DTO.CustomerBookTripDTO;
using TripBookingBE.Models;
using TripBookingBE.Services.ServiceInterfaces;

public class CustomerBookTripsService : ICustomerBookTripsService
{
    private readonly ICustomerBookTripsDal bookingDAL;

    public CustomerBookTripsService(ICustomerBookTripsDal bookingDAL)
    {
        this.bookingDAL = bookingDAL;
    }

    public async Task<CustomerBookTripDeleteByUserDTO> DeleteCustomerBookTripsByUser(long userId)
    {
        
        return await bookingDAL.DeleteCustomerBookTripsByUser(userId);
    }
}