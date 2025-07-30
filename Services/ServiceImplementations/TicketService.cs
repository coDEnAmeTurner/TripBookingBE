using TripBookingBE.Dal.DalImplementations;
using TripBookingBE.Dal.DalInterfaces;
using TripBookingBE.DTO.TicketDTO;
using TripBookingBE.Services.ServiceInterfaces;

namespace TripBookingBE.Services.ServiceImplementations;

public class TicketService : ITicketService
{
    private readonly ITicketDAL ticketDAL;

    public TicketService(ITicketDAL ticketDAL)
    {
        this.ticketDAL = ticketDAL;
    }

    // public async Task<TripCreateOrUpdateDTO> CreateOrUpdate(Trip trip)
    // {
    //     TripCreateOrUpdateDTO dto = new();

    //     if (trip.Id == 0)
    //     {

    //         dto = await tripDAL.Create(trip);
    //     }
    //     else
    //     {
    //         //check for password, with jwt configured
    //         dto = await tripDAL.Update(trip);
    //     }
    //     return dto;
    // }

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

    // public async Task<TripGetCreateOrUpdateDTO> GetCreateOrUpdateModel(long? id)
    // {
    //     TripGetCreateOrUpdateDTO dto = new();
    //     if (id == null)
    //         dto.Trip = new Models.Trip();
    //     else
    //     {
    //         var dtoDAL = await tripDAL.GetTripById(id.GetValueOrDefault());
    //         dto.Trip = dtoDAL.Trip;
    //         dto.StatusCode = dtoDAL.StatusCode;
    //         dto.Message = dtoDAL.Message;
    //     }

    //     return dto;
    // }

    // public async Task<TripGetByIdDTO> GetTripById(long id)
    // {
    //     var dto = await tripDAL.GetTripById(id);
    //     return dto;
    // }

    public async Task<TicketGetTicketsDTO> GetTickets(long? customerId, long? tripId, decimal? fromPrice, decimal? toPrice, string? sellerCode, DateTime? dateCreated, long? generalParamId)
    {
        var dto = await ticketDAL.GetTickets(customerId, tripId, fromPrice, toPrice, sellerCode, dateCreated, generalParamId);
        return dto;
    }

}