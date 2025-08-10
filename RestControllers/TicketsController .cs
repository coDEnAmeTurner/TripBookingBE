using System.Globalization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TripBookingBE.Pagination;
using TripBookingBE.RestRequests.Ticket;
using TripBookingBE.RestRequests.Trip;
using TripBookingBE.Services.ServiceInterfaces;

namespace TripBookingBE.RestControllers;

[Route("api/tickets")]
public class TicketsController : MyControllerBase
{
    private readonly ITicketService ticketService;

    public TicketsController(ITicketService ticketService)
    {
        this.ticketService = ticketService;
    }

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] TicketListRequest request)
    {
        var dto = await ticketService.GetTickets(
            request.CustomerId,
            request.TripId,
            request.FromPrice,
            request.ToPrice,
            request.SellerCode,
            DateTime.ParseExact(request.DepartureTime, "dd/MM/yyyy", CultureInfo.InvariantCulture),
            request.GeneralParamId
        );
        if (dto.RespCode != System.Net.HttpStatusCode.OK)
        {
            if (dto.RespCode == System.Net.HttpStatusCode.NotFound)
            {
                return NotFound(dto.Message);
            }
            return Problem(dto.Message);
        }

        int pageSize = 10;
        return Ok(await PaginatedList<Models.Ticket>.CreateAsync(dto.Tickets, request.PageNumber ?? 1, pageSize));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Details(int id)
    {
        var dto = await ticketService.GetTicketById(id);
        if (dto.RespCode != System.Net.HttpStatusCode.OK)
        {
            if (dto.RespCode == System.Net.HttpStatusCode.NotFound)
            {
                return NotFound(dto.Message);
            }
            return Problem(dto.Message);
        }

        return Ok(dto.Ticket);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var dto = await ticketService.DeleteTicket(id);
        if (dto.RespCode != System.Net.HttpStatusCode.NoContent)
        {
            if (dto.RespCode == System.Net.HttpStatusCode.NotFound)
            {
                return NotFound(dto.Message);
            }
            return Problem(dto.Message);
        }

        return NoContent();
    }

    [Authorize]
    [HttpPost]          
    public async Task<IActionResult> Create(TicketCreateRequest request)
    {
        var identity = HttpContext.User.Identity as ClaimsIdentity;
        var sellerCode = identity.Claims.FirstOrDefault(x => x.Type == "SellerCode").Value;
        var ticket = new Models.Ticket()
        {
            CustomerBookTripId = request.CustomerBookTripId.Value,
            CustomerId= request.CustomerId.Value,
            TripId=request.TripId.Value,
            Price=request.Price,
            SellerCode=sellerCode,
            GeneralParamId = request.GeneralParamId.Value

        };
        var dto = await ticketService.CreateOrUpdate(ticket);
        if (dto.RespCode != System.Net.HttpStatusCode.Created)
        {
            return Problem(dto.Message);
        }

        return Created($"api/trips/{ticket.CustomerBookTripId}", dto.Ticket);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] TicketCreateRequest request)
    {
        var dbdto = await ticketService.GetTicketById(id);
        if (dbdto.RespCode != System.Net.HttpStatusCode.OK)
        {
            return Problem(dbdto.Message);
        }
        var identity = HttpContext.User.Identity as ClaimsIdentity;
        var sellerCode = identity.Claims.FirstOrDefault(x => x.Type == "SellerCode").Value;
        var dbticket = dbdto.Ticket;
        
        dbticket.CustomerBookTripId = request.CustomerBookTripId.Value;
        dbticket.CustomerId = request.CustomerId.Value;
        dbticket.TripId = request.TripId.Value;
        dbticket.Price = request.Price;
        dbticket.SellerCode = sellerCode;
        dbticket.GeneralParamId = request.GeneralParamId;
        var dto = await ticketService.CreateOrUpdate(dbticket);
        if (dto.RespCode != System.Net.HttpStatusCode.Created)
        {
            return Problem(dto.Message);
        }

        return Ok(dto.Ticket);
    }

}