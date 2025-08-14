using System.Net;
using System.Transactions;
using TripBookingBE.Dal.DalImplementations;
using TripBookingBE.Dal.DalInterfaces;
using TripBookingBE.DTO.TicketDTO;
using TripBookingBE.Models;
using TripBookingBE.Services.ServiceInterfaces;

namespace TripBookingBE.Services.ServiceImplementations;

public class TicketService : ITicketService
{
    private readonly ITicketDAL ticketDAL;
    private readonly IBookingsDal bookingDAL;

    public TicketService(ITicketDAL ticketDAL, IBookingsDal bookingDAL)
    {
        this.ticketDAL = ticketDAL;
        this.bookingDAL = bookingDAL;
    }

    public async Task<TicketCheckOwnerDTO> CheckTicketOwner(long id, long userId)
    {
        var dto = new TicketCheckOwnerDTO();

        var ticketdto = await GetTicketById(id);
        if (ticketdto.Ticket == null || ticketdto.Ticket.CustomerBookTrip == null || ticketdto.Ticket.CustomerBookTrip.Customer == null || ticketdto.RespCode != HttpStatusCode.OK)
        {
            dto.RespCode = (int)ticketdto.RespCode;
            dto.Message = ticketdto.Message;
            return dto;
        }

        var ticket = ticketdto.Ticket;
        dto.IsOwner = userId == ticket.CustomerBookTrip.Customer.Id;
        return dto;
    }

    public async Task<TicketCheckSellerDTO> CheckTicketSeller(long id, string sellerCode)
    {
        var dto = new TicketCheckSellerDTO();

        var ticketdto = await GetTicketById(id);
        if (ticketdto.Ticket == null || ticketdto.RespCode != HttpStatusCode.OK)
        {
            dto.RespCode = (int)ticketdto.RespCode;
            dto.Message = ticketdto.Message;
            return dto;
        }

        var ticket = ticketdto.Ticket;
        dto.IsSeller = sellerCode == ticket.SellerCode;
        return dto;
    }

    public async Task<TicketCreateOrUpdateDTO> CreateOrUpdate(Ticket ticket)
    {
        TicketCreateOrUpdateDTO dto = new();

        if (ticket.CustomerBookTripId == 0)
        {
            var idDTO = await bookingDAL.GetIdByCustIdAndTripId(ticket.CustomerId, ticket.TripId);
            if (idDTO == null || idDTO.Ids == null || idDTO.Ids.Count == 0)
            {
                dto.Ticket = ticket;
                dto.RespCode = System.Net.HttpStatusCode.NotFound;
                dto.Message = $"Customer {ticket.CustomerId} doesn't book Trip {ticket.TripId}";
                return dto;
            }

            var already_exist = await ticketDAL.GetTicketById(idDTO.Ids.FirstOrDefault());
            if (already_exist.Ticket != null)
            {
                dto.Ticket = ticket;
                dto.RespCode = System.Net.HttpStatusCode.Conflict;
                dto.Message = $"{already_exist.Ticket.CustomerBookTrip?.Customer.Name} already has a Ticket for {already_exist.Ticket.CustomerBookTrip?.Trip.Route?.RouteDescription} - {already_exist.Ticket.CustomerBookTrip?.Trip.DepartureTime.GetValueOrDefault().ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture)}";
                return dto;
            }

            ticket.CustomerBookTripId = idDTO.Ids.FirstOrDefault();

            dto = await ticketDAL.Create(ticket);
        }
        else
        {
            dto = await ticketDAL.Update(ticket);
        }
        return dto;
    }

    public async Task<TicketDeleteDTO> DeleteTicket(long id)
    {
        TicketDeleteDTO dto = new();
        using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            try
            {

                var ticketDTO = await ticketDAL.DeleteTicket(id);
                if (ticketDTO.RespCode != HttpStatusCode.NoContent)
                {
                    dto.RespCode = ticketDTO.RespCode;
                    dto.Message += $"\n{ticketDTO.Message}";
                }

                await bookingDAL.DeleteBooking(ticketDTO.Ticket.CustomerBookTrip);

                scope.Complete();
            }
            catch (Exception ex)
            {
                if (dto.RespCode == HttpStatusCode.NoContent)
                {
                    dto.RespCode = HttpStatusCode.InternalServerError;
                    dto.Message = ex.Message;
                }
            }

        }
        return dto;
    }

    public async Task<TicketGetCreateOrUpdateDTO> GetCreateOrUpdateModel(long? id)
    {
        TicketGetCreateOrUpdateDTO dto = new();
        if (id == null)
            dto.Ticket = new Models.Ticket();
        else
        {
            var dtoDAL = await ticketDAL.GetTicketById(id.GetValueOrDefault());
            dto.Ticket = dtoDAL.Ticket;
            dto.RespCode = dtoDAL.RespCode;
            dto.Message = dtoDAL.Message;
        }

        return dto;
    }

    public async Task<TicketGetByIdDTO> GetTicketById(long id)
    {
        var dto = await ticketDAL.GetTicketById(id);
        return dto;
    }

    public async Task<TicketGetTicketsDTO> GetTickets(long? customerId, long? tripId, decimal? fromPrice, decimal? toPrice, string? sellerCode, DateTime? departureTime, long? generalParamId)
    {
        var dto = await ticketDAL.GetTickets(customerId, tripId, fromPrice, toPrice, sellerCode, departureTime, generalParamId);
        return dto;
    }

}