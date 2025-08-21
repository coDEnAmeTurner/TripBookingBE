using System.Globalization;
using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TripBookingBE.Pagination;
using TripBookingBE.RestRequests.Ticket;
using TripBookingBE.RestRequests.Trip;
using TripBookingBE.Services.ServiceInterfaces;
using TripBookingBE.Web.RestRequests.Ticket;

namespace TripBookingBE.RestControllers;

[Route("api/tickets")]
public class ApiTicketsController : MyControllerBase
{
    private readonly ITicketService ticketService;

    public ApiTicketsController(ITicketService ticketService)
    {
        this.ticketService = ticketService;
    }

    [Authorize(Policy = "AllowDriverOrSeller")]
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] TicketListRequest request)
    {
        var dto = await ticketService.GetTickets(
            request.CustomerId,
            request.TripId,
            request.FromPrice,
            request.ToPrice,
            request.SellerCode,
            string.IsNullOrEmpty(request.DepartureTime) ? null : DateTime.ParseExact(request.DepartureTime, "dd/MM/yyyy", CultureInfo.InvariantCulture),
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

    [Authorize(Policy = "AllowDriverOrTicketOwner")]
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

    [Authorize(Policy = "AllowTicketSellerOnly")]
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

    [Authorize(Policy = "AllowSellerOnly")]
    [HttpPost]
    public async Task<IActionResult> Create(TicketCreateRequest request)
    {
        var identity = HttpContext.User.Identity as ClaimsIdentity;
        var sellerCode = identity.Claims.FirstOrDefault(x => x.Type == "SellerCode").Value;
        var ticket = new Models.Ticket()
        {
            CustomerId = request.CustomerId.GetValueOrDefault(),
            TripId = request.TripId.GetValueOrDefault(),
            Price = request.Price,
            SellerCode = sellerCode,
            GeneralParamId = request.GeneralParamId.HasValue ? request.GeneralParamId.Value : null

        };
        var dto = await ticketService.CreateOrUpdate(ticket);
        if (dto.RespCode != System.Net.HttpStatusCode.Created)
        {
            return Problem(dto.Message);
        }

        return Created($"api/trips/{ticket.CustomerBookTripId}", dto.Ticket);
    }

    [Authorize(Policy = "AllowTicketSellerOnly")]
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

    [Authorize(Policy = "AllowDriverOrTicketOwner")]
    [HttpGet("{id:int}/pay")]
    public async Task<IActionResult> Pay(int id)
    {
        var dto = await ticketService.Pay(id);
        if (dto.RespCode != 200)
        {
            if (dto.RespCode == 404)
            {
                return NotFound(dto);
            }
            else
            {
                return Problem(dto.Message);
            }
        }

        return Ok(dto);
    }

    [AllowAnonymous]
    [HttpGet("IPN")]
    public async Task<IActionResult> IPN([FromQuery] TicketIPNRequest request)
    {
        var httpreq = HttpContext.Request;
        var dto = await ticketService.IPN(
            request.vnp_TmnCode,
            request.vnp_Amount,
            request.vnp_BankCode,
            request.vnp_BankTranNo,
            request.vnp_CardType,
            request.vnp_PayDate,
            request.vnp_OrderInfo,
            request.vnp_TransactionNo,
            request.vnp_ResponseCode,
            request.vnp_TransactionStatus,
            request.vnp_TxnRef,
            request.vnp_SecureHash,
            $"{httpreq.Host}{httpreq.Path}{httpreq.QueryString}"
        );

        if (dto.RespCode != 200)
        {
            return Problem(dto.Message);
        }

        return Ok(dto);
    }

    [AllowAnonymous]
    [HttpGet("ReturnUrl")]
    public async Task<IActionResult> ReturnUrl([FromQuery] TicketIPNRequest request)
    {
        var httpreq = HttpContext.Request;
        var dto = await ticketService.ReturnUrl(
            request.vnp_TmnCode,
            request.vnp_Amount,
            request.vnp_BankCode,
            request.vnp_BankTranNo,
            request.vnp_CardType,
            request.vnp_PayDate,
            request.vnp_OrderInfo,
            request.vnp_TransactionNo,
            request.vnp_ResponseCode,
            request.vnp_TransactionStatus,
            request.vnp_TxnRef,
            request.vnp_SecureHash,
            $"{httpreq.Host}{httpreq.Path}{httpreq.QueryString}"
        );

        return Ok(dto);
    }
}