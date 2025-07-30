using System.Net;
using Microsoft.AspNetCore.Mvc;
using TripBookingBE.DTO.TicketDTO;
using TripBookingBE.Models;
using TripBookingBE.Pagination;
using TripBookingBE.Services.ServiceInterfaces;

namespace TripBookingBE.Controllers;

public class TicketsController : Controller
{
    private readonly ITicketService ticketService;

    public TicketsController()
    {
    }

    public async Task<IActionResult> Index(long? customerId, long? tripId, decimal? fromPrice, decimal? toPrice, string? sellerCode, string? dateCreated, long? generalParamId, int? pageNumber)
    {
        TicketGetTicketsDTO dto = new();

        dto = await ticketService.GetTickets(customerId, tripId, fromPrice, toPrice, sellerCode, dateCreated == null ? null : DateTime.ParseExact(dateCreated, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture), generalParamId);

        if (dto.StatusCode != HttpStatusCode.OK)
        {
            ViewData["statusCode"] = dto.StatusCode;
            ViewData["errorMessage"] = dto.Message;
            return View();
        }


        int pageSize = 3;
        return View(await PaginatedList<Ticket>.CreateAsync(dto.Tickets, pageNumber ?? 1, pageSize));
    }

    // private async Task PopulateDropdownList()
    // {
    //     ViewBag.RouteOptions = new SelectList((await routeService.GetRoutes(null, null)).Routes, "Id", "RouteDescription");
    //     ViewBag.DriverOptions = new SelectList((await usersService.GetUsers(type: "DRIVER")).Users, "Id", "Name");
    // }

    // public async Task<IActionResult> CreateOrUpdate(long? id)
    // {
    //     var dto = await tripService.GetCreateOrUpdateModel(id);
    //     if (dto.StatusCode != HttpStatusCode.OK)
    //     {
    //         ViewData["statusCode"] = dto.StatusCode;
    //         ViewData["errorMessage"] = dto.Message;
    //         return View();
    //     }
    //     if (dto.Trip.Id != 0 && dto.Trip.DepartureTime.HasValue)
    //     {
    //         dto.Trip.DepartureTimeStr = dto.Trip.DepartureTime.Value.ToString("dd/MM/yyyy");
    //     }
    //     await PopulateDropdownList();
    //     return View(dto.Trip);
    // }

    // [HttpPost]
    // [ValidateAntiForgeryToken]
    // public async Task<IActionResult> CreateOrUpdate([Bind("Id,DepartureTimeStr,PlaceCount,RegistrationNumber,DriverId,RouteId,RowVersion")] Models.Trip trip)
    // {
    //     if (ModelState.IsValid)
    //     {
    //         trip.DepartureTime = DateTime.ParseExact(trip.DepartureTimeStr, "dd/MM/yyyy",
    //                                    System.Globalization.CultureInfo.InvariantCulture);
    //         TripCreateOrUpdateDTO targetTrip = new() { Trip = trip };

    //         targetTrip = await tripService.CreateOrUpdate(trip);
    //         if (targetTrip.StatusCode != HttpStatusCode.Created)
    //         {
    //             ViewData["statusCode"] = targetTrip.StatusCode;
    //             ViewData["errorMessage"] = targetTrip.Message;
    //             if (targetTrip.StatusCode == HttpStatusCode.Conflict)
    //                 ModelState.Remove("RowVersion");
    //             await PopulateDropdownList();
    //             return View(targetTrip.Trip);
    //         }
    //         return RedirectToAction(nameof(Index));
    //     }
    //     await PopulateDropdownList();
    //     return View(trip);
    // }

    // public async Task<IActionResult> Details(long? id)
    // {
    //     var dto = await tripService.GetTripById(id.GetValueOrDefault());
    //     await PopulateDropdownList();
    //     if (dto.StatusCode != HttpStatusCode.OK || dto.Trip == null)
    //     {
    //         ViewData["statusCode"] = dto.StatusCode;
    //         ViewData["errorMessage"] = dto.Message;
    //         return View("Index");
    //     }

    //     return View(dto.Trip);
    // }

    // public async Task<IActionResult> Delete(long id)
    // {
    //     var dto = await tripService.DeleteTrip(id);
    //     if (dto.StatusCode != HttpStatusCode.NoContent)
    //     {
    //         ViewData["statusCode"] = dto.StatusCode;
    //         ViewData["errorMessage"] = dto.Message;
    //     }

    //     return RedirectToAction(nameof(Index));
    // }
}