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
    public BookingsController(IBookingsService bookingService)
    {
        this.bookingService = bookingService;
    }

    public async Task<IActionResult> Index(string? customerName, string? registrationNumber, string? departureTimeStr, string? routeDescription, int? pageNumber)
    {
        BookingGetBookingsDTO dto = new();

        dto = await bookingService.GetBookings(customerName, registrationNumber, departureTimeStr==null?null:DateTime.ParseExact(departureTimeStr, "dd/MM/yyyy", CultureInfo.InvariantCulture), routeDescription);
        if (dto.StatusCode != HttpStatusCode.OK)
        {
            ViewData["statusCode"] = dto.StatusCode;
            ViewData["errorMessage"] = dto.Message;
            return View();
        }


        int pageSize = 3;
        return View(await PaginatedList<CustomerBookTrip>.CreateAsync(dto.Bookings, pageNumber ?? 1, pageSize));
    }

    // public async Task<IActionResult> CreateOrUpdate(long? id)
    // {
    //     var dto = await ticketService.GetCreateOrUpdateModel(id);
    //     await PopuplateSellerCode();
    //     await PopulateDropDownList();
    //     if (dto.StatusCode != HttpStatusCode.OK)
    //     {
    //         ViewData["statusCode"] = dto.StatusCode;
    //         ViewData["errorMessage"] = dto.Message;
    //         return View();
    //     }

    //     if (dto.Ticket.CustomerBookTripId != 0)
    //     {
    //         dto.Ticket.CustomerId = dto.Ticket.CustomerBookTrip.CustomerId;
    //         dto.Ticket.TripId = dto.Ticket.CustomerBookTrip.TripId;
    //     }
    //     return View(dto.Ticket);
    // }

    // [HttpPost]
    // [ValidateAntiForgeryToken]
    // public async Task<IActionResult> CreateOrUpdate([Bind("CustomerBookTripId,CustomerId,TripId,Price,SellerCode,RegistrationNumber,GeneralParamId,RowVersion")] Models.Ticket ticket)
    // {
    //     if (ModelState.IsValid)
    //     {
    //         TicketCreateOrUpdateDTO targetTicket = new() { Ticket = ticket };

    //         targetTicket = await ticketService.CreateOrUpdate(ticket);
    //         if (targetTicket.StatusCode != HttpStatusCode.Created)
    //         {
    //             ViewData["statusCode"] = targetTicket.StatusCode;
    //             ViewData["errorMessage"] = targetTicket.Message;
    //             if (targetTicket.StatusCode == HttpStatusCode.Conflict)
    //                 ModelState.Remove("RowVersion");
    //             await PopuplateSellerCode();
    //             await PopulateDropDownList();
    //             return View(targetTicket.Ticket);
    //         }
    //         return RedirectToAction(nameof(Index));
    //     }
    //     await PopuplateSellerCode();
    //     await PopulateDropDownList();
    //     return View(ticket);
    // }

    // public async Task<IActionResult> Details(long? id)
    // {
    //     var dto = await ticketService.GetTicketById(id.GetValueOrDefault());
    //     dto.Ticket.CustomerId = dto.Ticket.CustomerBookTrip.Customer.Id;
    //     dto.Ticket.TripId = dto.Ticket.CustomerBookTrip.Trip.Id;
    //     await PopuplateSellerCode();
    //     await PopulateDropDownList();
    //     if (dto.StatusCode != HttpStatusCode.OK || dto.Ticket == null)
    //     {
    //         ViewData["statusCode"] = dto.StatusCode;
    //         ViewData["errorMessage"] = dto.Message;
    //         return View("Index");
    //     }

    //     return View(dto.Ticket);
    // }

    // public async Task<IActionResult> Delete(long id)
    // {
    //     var dto = await ticketService.DeleteTicket(id);
    //     if (dto.StatusCode != HttpStatusCode.NoContent)
    //     {
    //         ViewData["statusCode"] = dto.StatusCode;
    //         ViewData["errorMessage"] = dto.Message;
    //     }

    //     return RedirectToAction(nameof(Index));
    // }

    // async Task PopuplateSellerCode()
    // {
    //     var sellers = from s in (await usersService.GetUsers(type: "SELLER")).Users select new { Id = s.Id, SellerCode = s.SellerCode };
    //     ViewBag.SellerCodeOptions = new SelectList(sellers, "SellerCode", "SellerCode");
    // }

    // async Task PopulateDropDownList()
    // {
    //     var trips = await tripService.GetTrips(null, null, null, null, null);
    //     var customers = await usersService.GetUsers(type: "CUSTOMER");
    //     var generalParams = await generalParamService.GetGeneralParams();

    //     var tripOptions = from t in trips.Trips select new { Id = t.Id, Description = $"{t.Route?.RouteDescription} - {t.RegistrationNumber}" };

    //     ViewBag.TripOptions = new SelectList(tripOptions, "Id", "Description");
    //     ViewBag.CustomerOptions = new SelectList(customers.Users, "Id", "Name");
    //     ViewBag.GeneralParamOptions = new SelectList(generalParams.GeneralParams, "Id", "ParamDescription");
    // }
}