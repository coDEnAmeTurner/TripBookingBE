using System.Data.Common;
using TripBookingBE.Dal.DalInterfaces;
using TripBookingBE.Data;
using TripBookingBE.DTO.CustomerReviewTripDTO;

public class CustomerReviewTripsDal : ICustomerReviewTripsDal
{
    private readonly TripBookingContext context;
    public CustomerReviewTripsDal(TripBookingContext context)
    {
        this.context = context;
    }

    public async Task<CustomerReviewTripDeleteByUserDTO> DeleteCustomerReviewTripsByUser(long userId)
    {
        CustomerReviewTripDeleteByUserDTO dto = new();

        try
        {
            var reviews = context.CustomerReviewTrips.Where(x => x.CustomerId == userId);
            context.CustomerReviewTrips.RemoveRange(reviews);
            await context.SaveChangesAsync();
            dto.CustomerReviewTrips = reviews;
        }
        catch (Exception ex)
        {
            dto.StatusCode = System.Net.HttpStatusCode.InternalServerError;
            dto.Message = ex.Message;
        }
        return dto;
    }
}