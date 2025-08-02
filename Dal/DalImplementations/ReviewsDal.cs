using System.Data.Common;
using System.Globalization;
using System.Net;
using Microsoft.EntityFrameworkCore;
using TripBookingBE.Dal.DalInterfaces;
using TripBookingBE.Data;
using TripBookingBE.DTO.ReviewDTO;
using TripBookingBE.Models;

public class ReviewsDal : IReviewsDal
{
    private readonly TripBookingContext context;
    public ReviewsDal(TripBookingContext context)
    {
        this.context = context;
    }

    public async Task<ReviewCreateOrUpdateDTO> Create(CustomerReviewTrip review)
    {
        ReviewCreateOrUpdateDTO dto = new();

        try
        {
            context.Add(review);
            await context.SaveChangesAsync();

        }
        catch (Exception ex)
        {
            dto.StatusCode = System.Net.HttpStatusCode.InternalServerError;
            dto.Message = $"{ex.Message}\n{ex.InnerException?.Message}";
        }
        finally
        {
            dto.Review = review;

        }

        return dto;
    }

    public async Task<ReviewDeleteByIdDTO> DeleteReview(CustomerReviewTrip review)
    {
        ReviewDeleteByIdDTO dto = new();

        try
        {
            context.CustomerReviewTrips.Remove(review);
            await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            dto.StatusCode = HttpStatusCode.InternalServerError;
            dto.Message = $"{ex.Message}\n{ex.InnerException?.Message}";
        }

        dto.Review = review;

        return dto;
    }

    public async Task<ReviewDeleteByUserDTO> DeleteReviewsByUser(long userId)
    {
        ReviewDeleteByUserDTO dto = new();

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

    public async Task<ReviewGetIdByCustomerIdAndTripIdDTO> GetIdByCustIdAndTripId(long? custId, long? tripId)
    {
        ReviewGetIdByCustomerIdAndTripIdDTO dto = new();
        try
        {
            var ids = from review in context.CustomerReviewTrips
                      where (tripId == null || tripId == review.TripId)
                      && (custId == null || custId == review.CustomerId)
                      select review.Id;
            var result = await ids.ToListAsync();

            dto.Ids = result;

        }
        catch (Exception ex)
        {
            dto.StatusCode = System.Net.HttpStatusCode.InternalServerError;
            dto.Message = $"{ex.Message}\n{ex.InnerException?.Message}";
        }

        return dto;
    }

    public async Task<ReviewGetByIdDTO> GetReviewById(long id)
    {
        ReviewGetByIdDTO dto = new();
        try
        {
            var review = await context.CustomerReviewTrips
            .Include(e => e.Customer)
            .Include(e => e.Trip)
                .ThenInclude(e => e.Route)
            .FirstOrDefaultAsync(x=>x.Id == id);
            if (review == null)
            {
                dto.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                dto.Message = $"Review with Id {id} not found!";
            }
            dto.Review = review;
        }
        catch (Exception ex)
        {
            dto.StatusCode = System.Net.HttpStatusCode.InternalServerError;
            dto.Message = ex.Message;
        }
        return dto;
    }

    public async Task<ReviewGetReviewsDTO> GetReviews(string? customerName, string? registrationNumber, DateTime? departureTime, string? routeDescription, string? content)
    {
        ReviewGetReviewsDTO dto = new();
        try
        {
            var reviews = from review in context.CustomerReviewTrips
                           where (customerName == null || review.Customer.Name.Contains(customerName))
                            && (registrationNumber == null ||
                                (review.Trip != null &&
                                review.Trip.RegistrationNumber != null &&
                                review.Trip.RegistrationNumber.Contains(registrationNumber)))
                            && (departureTime == null ||
                                (review.Trip != null
                                && review.Trip.DepartureTime.HasValue
                                && review.Trip.DepartureTime == departureTime))
                            && (routeDescription == null ||
                                (review.Trip != null
                                && review.Trip.Route != null
                                && review.Trip.Route.RouteDescription != null
                                && review.Trip.Route.RouteDescription.Contains(routeDescription)))
                            && (content == null ||
                                (review.Content != null
                                && review.Content.Contains(content)))
                           orderby review.DateCreated descending
                           select review;

            var list = await reviews.Include(b=>b.Trip).ThenInclude(t=>t.Route).Include(b=>b.Customer).ToListAsync();
            
            dto.Reviews = list;

        }
        catch (Exception ex)
        {
            dto.StatusCode = HttpStatusCode.InternalServerError;
            dto.Message = $"{ex.Message}\n{ex.InnerException?.Message}";
        }

        return dto;
    }

    public async Task<ReviewCreateOrUpdateDTO> Update(CustomerReviewTrip review)
    {
        ReviewCreateOrUpdateDTO dto = new();
        try
        {
            var currentState = context.Entry(review).State;
            context.Entry(review).State = EntityState.Modified;
            context.Update(review);
            await context.SaveChangesAsync();

        }
        catch (DbUpdateConcurrencyException ex)
        {
            dto.StatusCode = HttpStatusCode.Conflict;

            var exceptionEntry = ex.Entries.Single();
            var clientValues = (CustomerReviewTrip)exceptionEntry.Entity;
            var databaseEntry = exceptionEntry.GetDatabaseValues();
            if (databaseEntry == null)
            {
                dto.Message =
                    "Unable to save changes. The User was deleted by another user.";
            }
            else
            {
                var databaseValues = (CustomerReviewTrip)databaseEntry.ToObject();

                if (databaseValues.CustomerId != clientValues.CustomerId)
                {
                    dto.Message = $"Customer - Current value: {databaseValues.Customer.Name} - Phone: {databaseValues.Customer.Phone} - Email: {databaseValues.Customer.Email}";
                }
                if (databaseValues.TripId != clientValues.TripId) 
                {
                    dto.Message = $"Trip - Current value: {databaseValues.Trip.Route?.RouteDescription} - Departure Time: {databaseValues.Trip.DepartureTime?.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)} - Registration Number: {databaseValues.Trip.RegistrationNumber}";
                }

                dto.Message += "\nThe record you attempted to edit "
                        + "was modified by another user after you got the original value. The "
                        + "edit operation was canceled and the current values in the database "
                        + "have been displayed. If you still want to edit this record, click "
                        + "the Save button again. Otherwise click the Back to List hyperlink.";
                review.RowVersion = (byte[])databaseValues.RowVersion;
            }
        }
        catch (Exception ex)
        {
            dto.Message = ex.Message;
            dto.StatusCode = System.Net.HttpStatusCode.InternalServerError;
        }
        finally
        {
            dto.Review = review;

        }

        return dto;
    }
}