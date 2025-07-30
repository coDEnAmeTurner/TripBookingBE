using TripBookingBE.DTO.TicketDTO;
using TripBookingBE.Models;

namespace TripBookingBE.Services.ServiceInterfaces;

public interface ITicketService
{
    Task<TicketGetTicketsDTO> GetTickets(long? customerId, long? tripId, decimal? fromPrice, decimal? toPrice, string? sellerCode, DateTime? dateCreated, long? generalParamId);

    // Task<TripGetCreateOrUpdateDTO> GetCreateOrUpdateModel(long? id);

    // Task<TripCreateOrUpdateDTO> CreateOrUpdate(Trip trip);

    // Task<TripGetByIdDTO> GetTripById(long id);

    // Task<TripDeleteDTO> DeleteTrip(long id);

}