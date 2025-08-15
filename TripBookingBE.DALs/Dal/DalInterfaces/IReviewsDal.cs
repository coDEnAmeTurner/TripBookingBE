using TripBookingBE.DTO.ReviewDTO;
using TripBookingBE.Models;

namespace TripBookingBE.Dal.DalInterfaces;

public interface IReviewsDal
{
    public Task<ReviewDeleteByUserDTO> DeleteReviewsByUser(long userId);

    public Task<ReviewGetReviewsDTO> GetReviews(string? customerName, string? registrationNumber, DateTime? departureTime, string? routeDescription, string? content);

    public Task<ReviewGetByIdDTO> GetReviewById(long id);

    public Task<ReviewGetIdByCustomerIdAndTripIdDTO> GetIdByCustIdAndTripId(long? custId, long? tripId);

    public Task<ReviewCreateOrUpdateDTO> Update(CustomerReviewTrip review);

    public Task<ReviewCreateOrUpdateDTO> Create(CustomerReviewTrip review);
    public Task<ReviewDeleteByIdDTO> DeleteReview(CustomerReviewTrip review);


}