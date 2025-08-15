using TripBookingBE.DTO.ReviewDTO;
using TripBookingBE.Models;

namespace TripBookingBE.Services.ServiceInterfaces;

public interface IReviewService
{
    Task<ReviewDeleteByUserDTO> DeleteReviewsByUser(long userId);
    Task<ReviewGetCreateOrUpdateDTO> GetCreateOrUpdateModel(long? id);

    Task<ReviewGetReviewsDTO> GetReviews(string? customerName, string? registrationNumber, DateTime? departureTime, string? routeDescription, string? content);

    Task<ReviewCreateOrUpdateDTO> CreateOrUpdate(CustomerReviewTrip review);
    Task<ReviewGetByIdDTO> GetReviewById(long id);
    Task<ReviewDeleteByIdDTO> DeleteReview(long id);

}