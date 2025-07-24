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

    public async Task<IActionResult> Index(string description, string dateCreated, int? pageNumber)
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
}