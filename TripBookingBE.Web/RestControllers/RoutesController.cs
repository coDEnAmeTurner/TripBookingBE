using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TripBookingBE.Models;
using TripBookingBE.Pagination;
using TripBookingBE.RestRequests.Route;
using TripBookingBE.Services.ServiceInterfaces;

namespace TripBookingBE.RestControllers;

[Route("api/routes")]
public class ApiRoutesController : MyControllerBase
{
    private readonly IRouteService routeService;

    public ApiRoutesController(IRouteService routeService)
    {
        this.routeService = routeService;
    }

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] RouteListRequest request)
    {
        var dto = await routeService.GetRoutes(request.Description, DateTime.ParseExact(request.DateCreated, "dd/MM/yyyy", CultureInfo.InvariantCulture));
        if (dto.RespCode != System.Net.HttpStatusCode.OK)
        {
            return Problem(dto.Message);
        }

        int pageSize = 10;
        return Ok(await PaginatedList<Models.Route>.CreateAsync(dto.Routes, request.PageNumber ?? 1, pageSize));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Details(int id)
    {
        var dto = await routeService.GetRouteById(id);
        if (dto.RespCode != System.Net.HttpStatusCode.OK)
        {
            return Problem(dto.Message);
        }

        return Ok(dto.Route);
    }

    [Authorize(Policy = "AllowAdmin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var dto = await routeService.DeleteRoute(id);
        if (dto.RespCode != System.Net.HttpStatusCode.NoContent)
        {
            return Problem(dto.Message);
        }

        return NoContent();
    }

    [HttpPost]
    [Authorize(Policy = "AllowAdmin")]
    public async Task<IActionResult> Create([FromBody] RouteCreateRequest request)
    {
        var route = new Models.Route()
        {
            RouteDescription = request.RouteDescription
        };
        var dto = await routeService.CreateOrUpdate(route);
        if (dto.RespCode != System.Net.HttpStatusCode.Created)
        {
            return Problem(dto.Message);
        }

        return Created($"api/routes/{route.Id}", dto.Route);
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = "AllowAdmin")]
    public async Task<IActionResult> Update(int id, [FromBody] RouteCreateRequest request)
    {
        var dbdto = await routeService.GetRouteById(id);
        if (dbdto.RespCode != System.Net.HttpStatusCode.OK)
        {
            return Problem(dbdto.Message);
        }
        var dbroute = dbdto.Route;
        dbroute.RouteDescription = request.RouteDescription;
        var dto = await routeService.CreateOrUpdate(dbroute);
        if (dto.RespCode != System.Net.HttpStatusCode.Created)
        {
            return Problem(dto.Message);
        }

        return Ok(dto.Route);
    }
}