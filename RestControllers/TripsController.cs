using System.Globalization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TripBookingBE.Pagination;
using TripBookingBE.RestRequests.Trip;
using TripBookingBE.Services.ServiceImplementations;
using TripBookingBE.Services.ServiceInterfaces;

namespace TripBookingBE.RestControllers;

[Route("api/trips")]
public class TripsController : MyControllerBase
{
    private readonly ITripService tripService;

    public TripsController(ITripService tripService)
    {
        this.tripService = tripService;
    }

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] TripListRequest request)
    {
        var dto = await tripService.GetTrips(request.PlaceCount, request.RouteId, request.DriverId, request.RegistrationNumber, DateTime.ParseExact(request.DepartureTime, "dd/MM/yyyy", CultureInfo.InvariantCulture));
        if (dto.RespCode != System.Net.HttpStatusCode.OK)
        {
            return Problem(dto.Message);
        }

        int pageSize = 10;
        return Ok(await PaginatedList<Models.Trip>.CreateAsync(dto.Trips, request.PageNumber ?? 1, pageSize));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Details(int id)
    {
        var dto = await tripService.GetTripById(id);
        if (dto.RespCode != System.Net.HttpStatusCode.OK)
        {
            return Problem(dto.Message);
        }

        return Ok(dto.Trip);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var dto = await tripService.DeleteTrip(id);
        if (dto.RespCode != System.Net.HttpStatusCode.NoContent)
        {
            return Problem(dto.Message);
        }

        return NoContent();
    }

    [HttpPost]
    public async Task<IActionResult> Create(TripCreateRequest request)
    {
        var trip = new Models.Trip()
        {
            DepartureTime = DateTime.ParseExact(request.DepartureTime, "dd/MM/yyyy", CultureInfo.InvariantCulture),
            PlaceCount = request.PlaceCount,
            RegistrationNumber = request.RegistrationNumber,
            DriverId = request.DriverId,
            RouteId = request.RouteId,

        };
        var dto = await tripService.CreateOrUpdate(trip);
        if (dto.RespCode != System.Net.HttpStatusCode.Created)
        {
            return Problem(dto.Message);
        }

        return Created($"api/trips/{trip.Id}", dto.Trip);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] TripCreateRequest request)
    {
        var dbdto = await tripService.GetTripById(id);
        if (dbdto.RespCode != System.Net.HttpStatusCode.OK)
        {
            return Problem(dbdto.Message);
        }
        var dbtrip = dbdto.Trip;
        dbtrip.DepartureTime = DateTime.ParseExact(request.DepartureTime, "dd/MM/yyyy", CultureInfo.InvariantCulture);
        dbtrip.PlaceCount = request.PlaceCount;
        dbtrip.RegistrationNumber = request.RegistrationNumber;
        dbtrip.DriverId = request.DriverId;
        dbtrip.RouteId = request.RouteId;
        var dto = await tripService.CreateOrUpdate(dbtrip);
        if (dto.RespCode != System.Net.HttpStatusCode.Created)
        {
            return Problem(dto.Message);
        }

        return Ok(dto.Trip);
    }

    [HttpPatch("{id:int}/users")]
    public async Task<IActionResult> AssignDriver(int id, [FromBody] TripAssignDriverRequest request)
    {
        var dto = await tripService.AssignDriver(id, request.DriverId);
        if (dto.RespCode != System.Net.HttpStatusCode.Created)
        {
            if (dto.RespCode == System.Net.HttpStatusCode.NotFound)
            {
                return NotFound(dto.Message);
            }
            return Problem(dto.Message);
        }

        return Ok(dto);
    }

    [HttpPatch("{id:int}/routes")]
    public async Task<IActionResult> AssignRoute(int id, [FromBody] TripAssignRouteRequest request)
    {
        var dto = await tripService.AssignRoute(id, request.RouteId);
        if (dto.RespCode != System.Net.HttpStatusCode.Created)
        {
            if (dto.RespCode == System.Net.HttpStatusCode.NotFound)
            {
                return NotFound(dto.Message);
            }
            return Problem(dto.Message);
        }

        return Ok(dto);
    }

    [Authorize]
    [HttpPost("{id:int}/book")]                                 
    public async Task<IActionResult> Book(int id, [FromBody] TripBookRequest request)
    {
        var identity = HttpContext.User.Identity as ClaimsIdentity;
        var userId = int.Parse(identity.FindFirst("NameId").Value);
        var dto = await tripService.Book(id, userId, request.PlaceNumber.Value);
        if (dto.RespCode != System.Net.HttpStatusCode.Created)
        {
            if (dto.RespCode == System.Net.HttpStatusCode.NotFound)
            {
                return NotFound(dto.Message);
            }
            if (dto.RespCode == System.Net.HttpStatusCode.BadRequest)
            {
                return BadRequest(dto.Message);
            }
            return Problem(dto.Message);
        }

        return Ok(dto);
    }
}