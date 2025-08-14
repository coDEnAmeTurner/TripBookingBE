using TripBookingBE.DTO.TicketDTO;
using TripBookingBE.Models;

namespace TripBookingBE.Services.ServiceInterfaces;

public interface ITicketService
{
    Task<TicketGetTicketsDTO> GetTickets(long? customerId, long? tripId, decimal? fromPrice, decimal? toPrice, string? sellerCode, DateTime? departureTime, long? generalParamId);

    Task<TicketGetCreateOrUpdateDTO> GetCreateOrUpdateModel(long? id);

    Task<TicketCreateOrUpdateDTO> CreateOrUpdate(Ticket trip);

    Task<TicketGetByIdDTO> GetTicketById(long id);

    Task<TicketDeleteDTO> DeleteTicket(long id);

    Task<TicketCheckOwnerDTO> CheckTicketOwner(long id, long userId);
    Task<TicketCheckSellerDTO> CheckTicketSeller(long id, string sellerCode);

}