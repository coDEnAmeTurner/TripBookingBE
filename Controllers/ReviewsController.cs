using System.Globalization;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TripBookingBE.DTO.ReviewDTO;
using TripBookingBE.Models;
using TripBookingBE.Pagination;
using TripBookingBE.Services.ServiceInterfaces;

namespace TripBookingBE.Controllers;

public class ReviewsController : Controller
{
    private readonly IReviewService reviewService;
    private readonly IUsersService userService;
    private readonly ITripService tripService;

    public ReviewsController(IReviewService reviewService, IUsersService userService, ITripService tripService)
    {
        this.reviewService = reviewService;
        this.userService = userService;
        this.tripService = tripService;
    }

    public async Task<IActionResult> Index(string? customerName, string? registrationNumber, string? departureTimeStr, string? routeDescription, string? content, int? pageNumber)
    {
        ReviewGetReviewsDTO dto = new();

        dto = await reviewService.GetReviews(customerName, registrationNumber, departureTimeStr == null ? null : DateTime.ParseExact(departureTimeStr, "dd/MM/yyyy", CultureInfo.InvariantCulture), routeDescription, content);
        if (dto.StatusCode != HttpStatusCode.OK)
        {
            ViewData["statusCode"] = dto.StatusCode;
            ViewData["errorMessage"] = dto.Message;
            return View();
        }


        int pageSize = 3;
        return View(await PaginatedList<CustomerReviewTrip>.CreateAsync(dto.Reviews, pageNumber ?? 1, pageSize));
    }

    public async Task<IActionResult> CreateOrUpdate(long? id)
    {
        var dto = await reviewService.GetCreateOrUpdateModel(id);
        await PopulateDropDownList();
        if (dto.StatusCode != HttpStatusCode.OK)
        {
            ViewData["statusCode"] = dto.StatusCode;
            ViewData["errorMessage"] = dto.Message;
            return View();
        }

        return View(dto.Review);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateOrUpdate([Bind("Id,CustomerId,TripId,Content,RowVersion")] Models.CustomerReviewTrip review)
    {
        if (ModelState.IsValid)
        {
            ReviewCreateOrUpdateDTO targetReview = new() { Review = review };

            targetReview = await reviewService.CreateOrUpdate(review);
            if (targetReview.StatusCode != HttpStatusCode.Created)
            {
                ViewData["statusCode"] = targetReview.StatusCode;
                ViewData["errorMessage"] = targetReview.Message;
                if (targetReview.StatusCode == HttpStatusCode.Conflict)
                    ModelState.Remove("RowVersion");
                await PopulateDropDownList();
                return View(targetReview.Review);
            }
            return RedirectToAction(nameof(Index));
        }
        await PopulateDropDownList();
        return View(review);
    }

    public async Task<IActionResult> Details(long? id)
    {
        var dto = await reviewService.GetReviewById(id.GetValueOrDefault());
        await PopulateDropDownList();
        if (dto.StatusCode != HttpStatusCode.OK || dto.Review == null)
        {
            ViewData["statusCode"] = dto.StatusCode;
            ViewData["errorMessage"] = dto.Message;
            return View("Index");
        }

        return View(dto.Review);
    }

    public async Task<IActionResult> Delete(long id)
    {
        var dto = await reviewService.DeleteReview(id);
        if (dto.StatusCode != HttpStatusCode.NoContent)
        {
            ViewData["statusCode"] = dto.StatusCode;
            ViewData["errorMessage"] = dto.Message;
        }

        return RedirectToAction(nameof(Index));
    }
    
    async Task PopulateDropDownList()
    {
        var trips = await tripService.GetTrips(null, null, null, null, null);
        var customers = await userService.GetUsers(type: "CUSTOMER");

        var tripOptions = from t in trips.Trips select new { Id = t.Id, Description = $"{t.Route?.RouteDescription} - {t.RegistrationNumber}" };

        ViewBag.TripOptions = new SelectList(tripOptions, "Id", "Description");
        ViewBag.CustomerOptions = new SelectList(customers.Users, "Id", "Name");
    }
}