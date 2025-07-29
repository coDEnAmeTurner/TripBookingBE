using System.Globalization;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using TripBookingBE.DTO.RouteDTO;
using TripBookingBE.DTO.TripDTO;
using TripBookingBE.Models;
using TripBookingBE.Pagination;
using TripBookingBE.Services.ServiceInterfaces;

namespace TripBookingBE.Controllers;

public class TripsController : Controller
{
    private readonly ITripService tripService;

    public TripsController(ITripService tripService)
    {
        this.tripService = tripService;
    }

    public async Task<IActionResult> Index(int? placeCount, int? routeId, int? driverId, string? registrationNumber, string departureTime, int? pageNumber)
    {
        TripGetTripsDTO dto = new();

        dto = await tripService.GetTrips(placeCount, routeId, driverId, registrationNumber, String.IsNullOrEmpty(departureTime) ? null : DateTime.ParseExact(departureTime, "dd/MM/yyyy",
                                       System.Globalization.CultureInfo.InvariantCulture), pageNumber);

        if (dto.StatusCode != HttpStatusCode.OK)
        {
            ViewData["statusCode"] = dto.StatusCode;
            ViewData["errorMessage"] = dto.Message;
            return View();
        }

        int pageSize = 3;
        return View(await PaginatedList<Trip>.CreateAsync(dto.Trips, pageNumber ?? 1, pageSize));
    }

    public async Task<IActionResult> CreateOrUpdate(long? id)
    {
        var dto = await routeService.GetCreateOrUpdateModel(id);
        if (dto.StatusCode != HttpStatusCode.OK)
        {
            ViewData["statusCode"] = dto.StatusCode;
            ViewData["errorMessage"] = dto.Message;
            return View();
        }
        return View(dto.Route);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateOrUpdate([Bind("Id,RouteDescription,RowVersion")] Models.Route route)
    {
        if (ModelState.IsValid)
        {
            RouteCreateOrUpdateDTO targetRoute = new() { Route = route };

            targetRoute = await routeService.CreateOrUpdate(route);
            if (targetRoute.StatusCode != HttpStatusCode.Created)
            {
                ViewData["statusCode"] = targetRoute.StatusCode;
                ViewData["errorMessage"] = targetRoute.Message;
                if (targetRoute.StatusCode == HttpStatusCode.Conflict)
                    ModelState.Remove("RowVersion");
                return View(targetRoute.Route);
            }
            return RedirectToAction(nameof(Index));
        }
        return View(route);
    }

    public async Task<IActionResult> Details(long? id)
    {
        var dto = await routeService.GetRouteById(id.GetValueOrDefault());
        if (dto.StatusCode != HttpStatusCode.OK || dto.Route == null)
        {
            ViewData["statusCode"] = dto.StatusCode;
            ViewData["errorMessage"] = dto.Message;
            return View("Index");
        }

        return View(dto.Route);
    }

    public async Task<IActionResult> Delete(long id)
    {
        var dto = await routeService.DeleteRoute(id);
        if (dto.StatusCode != HttpStatusCode.NoContent)
        {
            ViewData["statusCode"] = dto.StatusCode;
            ViewData["errorMessage"] = dto.Message;
        }

        return RedirectToAction(nameof(Index));
    }
}