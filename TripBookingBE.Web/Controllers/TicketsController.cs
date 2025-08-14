using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TripBookingBE.DTO.TicketDTO;
using TripBookingBE.Models;
using TripBookingBE.Pagination;
using TripBookingBE.Services.ServiceImplementations;
using TripBookingBE.Services.ServiceInterfaces;

namespace TripBookingBE.Controllers;

public class TicketsController : Controller
{
    private readonly ITicketService ticketService;
    private readonly ITripService tripService;
    private readonly IUsersService usersService;
    private readonly IGeneralParamService generalParamService;

    public TicketsController(ITicketService ticketService, ITripService tripService, IUsersService usersService, IGeneralParamService generalParamService)
    {
        this.ticketService = ticketService;
        this.tripService = tripService;
        this.usersService = usersService;
        this.generalParamService = generalParamService;
    }

    public async Task<IActionResult> Index(long? customerId, long? tripId, decimal? fromPrice, decimal? toPrice, string? sellerCode, string? departureTime, long? generalParamId, int? pageNumber)
    {
        TicketGetTicketsDTO dto = new();

        dto = await ticketService.GetTickets(customerId, tripId, fromPrice, toPrice, sellerCode, departureTime == null ? null : DateTime.ParseExact(departureTime, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture), generalParamId);
        await PopulateDropDownList();
        if (dto.RespCode != HttpStatusCode.OK)
        {
            ViewData["statusCode"] = dto.RespCode;
            ViewData["errorMessage"] = dto.Message;
            return View();
        }


        int pageSize = 3;
        return View(await PaginatedList<Ticket>.CreateAsync(dto.Tickets, pageNumber ?? 1, pageSize));
    }

    public async Task<IActionResult> CreateOrUpdate(long? id)
    {
        var dto = await ticketService.GetCreateOrUpdateModel(id);
        await PopuplateSellerCode();
        await PopulateDropDownList();
        if (dto.RespCode != HttpStatusCode.OK)
        {
            ViewData["statusCode"] = dto.RespCode;
            ViewData["errorMessage"] = dto.Message;
            return View();
        }

        if (dto.Ticket.CustomerBookTripId != 0)
        {
            dto.Ticket.CustomerId = dto.Ticket.CustomerBookTrip.CustomerId;
            dto.Ticket.TripId = dto.Ticket.CustomerBookTrip.TripId;
        }
        return View(dto.Ticket);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateOrUpdate([Bind("CustomerBookTripId,CustomerId,TripId,Price,SellerCode,RegistrationNumber,GeneralParamId,RowVersion")] Models.Ticket ticket)
    {
        if (ModelState.IsValid)
        {
            TicketCreateOrUpdateDTO targetTicket = new() { Ticket = ticket };

            targetTicket = await ticketService.CreateOrUpdate(ticket);
            if (targetTicket.RespCode != HttpStatusCode.Created)
            {
                ViewData["statusCode"] = targetTicket.RespCode;
                ViewData["errorMessage"] = targetTicket.Message;
                if (targetTicket.RespCode == HttpStatusCode.Conflict)
                    ModelState.Remove("RowVersion");
                await PopuplateSellerCode();
                await PopulateDropDownList();
                return View(targetTicket.Ticket);
            }
            return RedirectToAction(nameof(Index));
        }
        await PopuplateSellerCode();
        await PopulateDropDownList();
        return View(ticket);
    }

    public async Task<IActionResult> Details(long? id)
    {
        var dto = await ticketService.GetTicketById(id.GetValueOrDefault());
        dto.Ticket.CustomerId = dto.Ticket.CustomerBookTrip.Customer.Id;
        dto.Ticket.TripId = dto.Ticket.CustomerBookTrip.Trip.Id;
        await PopuplateSellerCode();
        await PopulateDropDownList();
        if (dto.RespCode != HttpStatusCode.OK || dto.Ticket == null)
        {
            ViewData["statusCode"] = dto.RespCode;
            ViewData["errorMessage"] = dto.Message;
            return View("Index");
        }

        return View(dto.Ticket);
    }

    public async Task<IActionResult> Delete(long id)
    {
        var dto = await ticketService.DeleteTicket(id);
        if (dto.RespCode != HttpStatusCode.NoContent)
        {
            ViewData["statusCode"] = dto.RespCode;
            ViewData["errorMessage"] = dto.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    async Task PopuplateSellerCode()
    {
        var sellers = from s in (await usersService.GetUsers(type: "SELLER")).Users select new { Id = s.Id, SellerCode = s.SellerCode };
        ViewBag.SellerCodeOptions = new SelectList(sellers, "SellerCode", "SellerCode");
    }

    async Task PopulateDropDownList()
    {
        var trips = await tripService.GetTrips(null, null, null, null, null);
        var customers = await usersService.GetUsers(type: "CUSTOMER");
        var generalParams = await generalParamService.GetGeneralParams();

        var tripOptions = from t in trips.Trips select new { Id = t.Id, Description = $"{t.Route?.RouteDescription} - {t.RegistrationNumber}" };

        ViewBag.TripOptions = new SelectList(tripOptions, "Id", "Description");
        ViewBag.CustomerOptions = new SelectList(customers.Users, "Id", "Name");
        ViewBag.GeneralParamOptions = new SelectList(generalParams.GeneralParams, "Id", "ParamDescription");
    }
}