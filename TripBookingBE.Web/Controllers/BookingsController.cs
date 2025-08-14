using System.Globalization;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TripBookingBE.DTO.BookingDTO;
using TripBookingBE.Models;
using TripBookingBE.Pagination;
using TripBookingBE.Services.ServiceInterfaces;

namespace TripBookingBE.Controllers;

public class BookingsController : Controller
{
    private readonly IBookingsService bookingService;
    private readonly ITripService tripService;
    private readonly IUsersService usersService;
    public BookingsController(IBookingsService bookingService, ITripService tripService, IUsersService usersService)
    {
        this.bookingService = bookingService;
        this.tripService = tripService;
        this.usersService = usersService;
    }

    public async Task<IActionResult> Index(string? customerName, string? registrationNumber, string? departureTimeStr, string? routeDescription, int? pageNumber)
    {
        BookingGetBookingsDTO dto = new();

        dto = await bookingService.GetBookings(customerName, registrationNumber, departureTimeStr == null ? null : DateTime.ParseExact(departureTimeStr, "dd/MM/yyyy", CultureInfo.InvariantCulture), routeDescription);
        if (dto.RespCode != HttpStatusCode.OK)
        {
            ViewData["statusCode"] = dto.RespCode;
            ViewData["errorMessage"] = dto.Message;
            return View();
        }


        int pageSize = 3;
        return View(await PaginatedList<CustomerBookTrip>.CreateAsync(dto.Bookings, pageNumber ?? 1, pageSize));
    }

    public async Task<IActionResult> CreateOrUpdate(long? id)
    {
        var dto = await bookingService.GetCreateOrUpdateModel(id);
        await PopulateDropDownList();
        if (dto.RespCode != HttpStatusCode.OK)
        {
            ViewData["statusCode"] = dto.RespCode;
            ViewData["errorMessage"] = dto.Message;
            return View();
        }

        return View(dto.CustomerBookTrip);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateOrUpdate([Bind("Id,CustomerId,TripId,PlaceNumber,RowVersion")] Models.CustomerBookTrip booking)
    {
        if (ModelState.IsValid)
        {
            BookingCreateOrUpdateDTO targetBooking = new() { CustomerBookTrip = booking };

            targetBooking = await bookingService.CreateOrUpdate(booking);
            if (targetBooking.RespCode != HttpStatusCode.Created)
            {
                ViewData["statusCode"] = targetBooking.RespCode;
                ViewData["errorMessage"] = targetBooking.Message;
                if (targetBooking.RespCode == HttpStatusCode.Conflict)
                    ModelState.Remove("RowVersion");
                await PopulateDropDownList();
                return View(targetBooking.CustomerBookTrip);
            }
            return RedirectToAction(nameof(Index));
        }
        await PopulateDropDownList();
        return View(booking);
    }

    public async Task<IActionResult> Details(long? id)
    {
        var dto = await bookingService.GetBookingById(id.GetValueOrDefault());
        await PopulateDropDownList();
        if (dto.RespCode != HttpStatusCode.OK || dto.CustomerBookTrip == null)
        {
            ViewData["statusCode"] = dto.RespCode;
            ViewData["errorMessage"] = dto.Message;
            return View("Index");
        }

        return View(dto.CustomerBookTrip);
    }

    public async Task<IActionResult> Delete(long id)
    {
        var dto = await bookingService.DeleteBooking(id);
        if (dto.RespCode != HttpStatusCode.NoContent)
        {
            ViewData["statusCode"] = dto.RespCode;
            ViewData["errorMessage"] = dto.Message;
        }

        return RedirectToAction(nameof(Index));
    }
    
    async Task PopulateDropDownList()
    {
        var trips = await tripService.GetTrips(null, null, null, null, null);
        var customers = await usersService.GetUsers(type: "CUSTOMER");

        var tripOptions = from t in trips.Trips select new { Id = t.Id, Description = $"{t.Route?.RouteDescription} - {t.RegistrationNumber}" };

        ViewBag.TripOptions = new SelectList(tripOptions, "Id", "Description");
        ViewBag.CustomerOptions = new SelectList(customers.Users, "Id", "Name");
    }
}