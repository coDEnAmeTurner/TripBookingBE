using Microsoft.AspNetCore.Mvc;
using TripBookingBE.RestRequests.Route;
using TripBookingBE.Services.ServiceInterfaces;

namespace TripBookingBE.RestControllers;

[Route("api/routes")]
public class RoutesRestController : MyControllerBase
{
    private readonly IRouteService routeService;

    public RoutesRestController(IRouteService routeService)
    {
        this.routeService = routeService;
    }

    [HttpGet]
    public async Task<IActionResult> List(RouteListRequest request)
    {
        var dto = await routeService.GetRoutes(request.Description, );
        return null;
    }
}