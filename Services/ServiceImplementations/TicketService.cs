using TripBookingBE.Dal.DalImplementations;
using TripBookingBE.Dal.DalInterfaces;
using TripBookingBE.DTO.TicketDTO;
using TripBookingBE.Models;
using TripBookingBE.Services.ServiceInterfaces;

namespace TripBookingBE.Services.ServiceImplementations;

public class TicketService : ITicketService
{
    private readonly ITicketDAL ticketDAL;

    public TicketService(ITicketDAL ticketDAL)
    {
        this.ticketDAL = ticketDAL;
    }

    public async Task<TicketCreateOrUpdateDTO> CreateOrUpdate(Ticket ticket)
    {
        TicketCreateOrUpdateDTO dto = new();

        if (ticket.CustomerBookTripId == 0)
        {

            dto = await ticketDAL.Create(ticket);
        }
        else
        {
            dto = await ticketDAL.Update(ticket);
        }
        return dto;
    }

    // public async Task<TripDeleteDTO> DeleteTrip(long id)
    // {
    //     TripDeleteDTO dto = new();
    //     using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
    //     {
    //         try
    //         {

    //             var tripDTO = await tripDAL.DeleteTrip(id);
    //             if (tripDTO.StatusCode != HttpStatusCode.NoContent)
    //             {
    //                 dto.StatusCode = tripDTO.StatusCode;
    //                 dto.Message += $"\n{tripDTO.Message}";
    //             }

    //             scope.Complete();
    //         }
    //         catch (Exception ex)
    //         {
    //             if (dto.StatusCode == HttpStatusCode.NoContent)
    //             {
    //                 dto.StatusCode = HttpStatusCode.InternalServerError;
    //                 dto.Message = ex.Message;
    //             }
    //         }

    //     }
    //     return dto;
    // }

    public async Task<TicketGetCreateOrUpdateDTO> GetCreateOrUpdateModel(long? id)
    {
        TicketGetCreateOrUpdateDTO dto = new();
        if (id == null)
            dto.Ticket = new Models.Ticket();
        else
        {
            var dtoDAL = await ticketDAL.GetTicketById(id.GetValueOrDefault());
            dto.Ticket = dtoDAL.Ticket;
            dto.StatusCode = dtoDAL.StatusCode;
            dto.Message = dtoDAL.Message;
        }

        return dto;
    }

    // public async Task<TripGetByIdDTO> GetTripById(long id)
    // {
    //     var dto = await tripDAL.GetTripById(id);
    //     return dto;
    // }

    public async Task<TicketGetTicketsDTO> GetTickets(long? customerId, long? tripId, decimal? fromPrice, decimal? toPrice, string? sellerCode, DateTime? departureTime, long? generalParamId)
    {
        var dto = await ticketDAL.GetTickets(customerId, tripId, fromPrice, toPrice, sellerCode, departureTime, generalParamId);
        return dto;
    }

}