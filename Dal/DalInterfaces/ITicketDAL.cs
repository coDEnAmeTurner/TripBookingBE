using TripBookingBE.DTO.TicketDTO;

namespace  TripBookingBE.Dal.DalInterfaces;

public interface ITicketDAL
{
    public Task<TicketGetTicketsDTO> GetTickets(long? customerId, long? tripId, decimal? fromPrice, decimal? toPrice, string? sellerCode, DateTime? departureTime, long? generalParamId);

    public Task<TicketGetByIdDTO> GetTicketById(long id);

    public Task<TicketCreateOrUpdateDTO> Create(Models.Ticket ticket);

    public Task<TicketCreateOrUpdateDTO> Update(Models.Ticket ticket);

    public Task<TicketDeleteDTO> DeleteTicket(long id);
}