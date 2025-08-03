using System.Globalization;
using TripBookingBE.Dal.DalImplementations;
using TripBookingBE.Dal.DalInterfaces;
using TripBookingBE.DTO.ReviewDTO;
using TripBookingBE.Models;
using TripBookingBE.Services.ServiceInterfaces;

public class ReviewsService : IReviewService
{
    private readonly IReviewsDal reviewDAL;

    public ReviewsService(IReviewsDal reviewDAL)
    {
        this.reviewDAL = reviewDAL;
    }

    public async Task<ReviewCreateOrUpdateDTO> CreateOrUpdate(CustomerReviewTrip review)
    {
        ReviewCreateOrUpdateDTO dto = new();

        if (review.Id == 0)
        {
            dto = await reviewDAL.Create(review);
        }
        else
        {
            //check for password, with jwt configured

            dto = await reviewDAL.Update(review);
        }
        return dto;
    }

    public async Task<ReviewDeleteByIdDTO> DeleteReview(long id)
    {
        var review = await reviewDAL.GetReviewById(id);
        if (review == null)
        {
            ReviewDeleteByIdDTO dto = new();
            dto.StatusCode = System.Net.HttpStatusCode.NotFound;
            dto.Message = "The review is not found!";
            return dto;
        }

        return await reviewDAL.DeleteReview(review.Review);
    }

    public async Task<ReviewDeleteByUserDTO> DeleteReviewsByUser(long userId)
    {
        return await reviewDAL.DeleteReviewsByUser(userId);
    }

    public async Task<ReviewGetCreateOrUpdateDTO> GetCreateOrUpdateModel(long? id)
    {
        ReviewGetCreateOrUpdateDTO dto = new();

        if (id == null)
        {
            dto.Review = new();
        }
        else
        {
            var dtobyid = await reviewDAL.GetReviewById(id.GetValueOrDefault());
            dto.StatusCode = dtobyid.StatusCode;
            dto.Message = dtobyid.Message;
            dto.Review = dtobyid.Review;

        }

        return dto;
    }

    public async Task<ReviewGetByIdDTO> GetReviewById(long id)
    {
        return await reviewDAL.GetReviewById(id);

    }

    public async Task<ReviewGetReviewsDTO> GetReviews(string? customerName, string? registrationNumber, DateTime? departureTime, string? routeDescription, string? content)
    {
        return await reviewDAL.GetReviews(customerName, registrationNumber, departureTime, routeDescription, content);
    }
}