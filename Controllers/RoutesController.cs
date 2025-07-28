using System.Globalization;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using TripBookingBE.DTO.RouteDTO;
using TripBookingBE.Models;
using TripBookingBE.Pagination;
using TripBookingBE.Services.ServiceInterfaces;

namespace TripBookingBE.Controllers;

public class RoutesController : Controller
{
    private readonly IRouteService routeService;

    public RoutesController(IRouteService routeService)
    {
        this.routeService = routeService;
    }

    public async Task<IActionResult> Index(string? description, string dateCreated, int? pageNumber)
    {
        RouteGetRoutesDTO dto = new();

        dto = await routeService.GetRoutes(description, String.IsNullOrEmpty(dateCreated) ? null : DateTime.ParseExact(dateCreated, "dd/MM/yyyy",
                                       System.Globalization.CultureInfo.InvariantCulture));

        if (dto.StatusCode != HttpStatusCode.OK)
        {
            ViewData["statusCode"] = dto.StatusCode;
            ViewData["errorMessage"] = dto.Message;
            return View();
        }

        int pageSize = 3;
        return View(await PaginatedList<Models.Route>.CreateAsync(dto.Routes, pageNumber ?? 1, pageSize));
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