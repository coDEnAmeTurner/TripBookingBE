using TripBookingBE.DTO.TicketDTO;

namespace  TripBookingBE.Dal.DalInterfaces;

public interface ITicketDAL
{
    public Task<TicketGetTicketsDTO> GetTickets(long? customerId, long? tripId, decimal? fromPrice, decimal? toPrice, string? sellerCode, DateTime? dateCreated, long? generalParamId);

    // public Task<TripGetByIdDTO> GetTripById(long id);

    // public Task<TripCreateOrUpdateDTO> Create(Models.Trip trip);

    // public Task<TripCreateOrUpdateDTO> Update(Models.Trip trip);

    // public Task<TripDeleteDTO> DeleteTrip(long id);
}