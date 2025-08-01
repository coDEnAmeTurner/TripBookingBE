using System.Net;
using Microsoft.EntityFrameworkCore;
using TripBookingBE.Dal.DalInterfaces;
using TripBookingBE.Data;
using TripBookingBE.DTO.TicketDTO;
using TripBookingBE.Models;

namespace TripBookingBE.Dal.DalImplementations;

public class TicketDAL : ITicketDAL
{
    private readonly TripBookingContext context;

    private readonly ICustomerBookTripsDal customerBookTripsDal;
    private readonly IGeneralParamDal generalParamDal;

    public TicketDAL(TripBookingContext context, ICustomerBookTripsDal customerBookTripsDal, IGeneralParamDal generalParamDal)
    {
        this.context = context;
        this.customerBookTripsDal = customerBookTripsDal;
        this.generalParamDal = generalParamDal;
    }

    public async Task<TicketCreateOrUpdateDTO> Create(Ticket ticket)
    {
        TicketCreateOrUpdateDTO dto = new();

        try
        {
            context.Add(ticket);
            await context.SaveChangesAsync();

        }
        catch (Exception ex)
        {
            dto.StatusCode = System.Net.HttpStatusCode.InternalServerError;
            dto.Message = $"{ex.Message}\n{ex.InnerException?.Message}";
        }
        finally
        {
            dto.Ticket = ticket;

        }

        return dto;
    }

    // public async Task<TripDeleteDTO> DeleteTrip(long id)
    // {
    //     TripDeleteDTO dto = new();

    //     var inst = await context.Trips.FindAsync(id);
    //     if (inst == null)
    //     {
    //         dto.StatusCode = System.Net.HttpStatusCode.NotFound;
    //         dto.Message += $"\nTrip with Id {id} not found!";
    //     }

    //     context.Trips.Remove(inst);
    //     await context.SaveChangesAsync();

    //     dto.Trip = inst;
    //     return dto;
    // }

    public async Task<TicketGetTicketsDTO> GetTickets(long? customerId, long? tripId, decimal? fromPrice, decimal? toPrice, string? sellerCode, DateTime? departureTime, long? generalParamId)
    {
        TicketGetTicketsDTO dto = new();
        try
        {

            var tickets = from t in context.Tickets select t;
            tickets = from t in tickets where customerId == null || customerId == 0 || t.CustomerBookTrip.CustomerId == customerId select t;
            tickets = from t in tickets where tripId == null || tripId == 0 || t.CustomerBookTrip.TripId == tripId select t;
            tickets = from t in tickets where generalParamId == null || generalParamId == 0 || t.GeneralParamId == generalParamId select t;
            tickets = from t in tickets where (fromPrice == null && toPrice == null)|| (fromPrice == 0 && toPrice == 0) || (t.Price >= fromPrice && t.Price <= toPrice) select t;
            tickets = from t in tickets
                      where departureTime == null || (
                t.DateCreated != null && t.DateCreated.HasValue &&
                t.DateCreated.Value.Year == departureTime.Value.Year &&
                t.DateCreated.Value.Month == departureTime.Value.Month &&
                t.DateCreated.Value.Date == departureTime.Value.Date

                )
                      orderby t.DateCreated descending
                      select t;
            var list_tickets = await tickets
            .Include(t => t.GeneralParam)
            .Include(t => t.CustomerBookTrip)
                .ThenInclude(t => t.Customer)
            .Include(t => t.CustomerBookTrip)
                .ThenInclude(t => t.Trip)
                    .ThenInclude(e => e.Route)
            .ToListAsync();
            list_tickets = list_tickets.Where(t => String.IsNullOrEmpty(sellerCode) || (t.SellerCode != null && t.SellerCode.Contains(sellerCode))).ToList();
            dto.Tickets = list_tickets;
        }
        catch (Exception ex)
        {
            dto.StatusCode = HttpStatusCode.InternalServerError;
            dto.Message = ex.Message;
        }

        return dto;

    }

    public async Task<TicketGetByIdDTO> GetTicketById(long id)
    {
        TicketGetByIdDTO dto = new();
        try
        {
            var ticket = await context.Tickets
            .Where(x => x.CustomerBookTripId == id)
            .Include(x => x.CustomerBookTrip)
                .ThenInclude(x => x.Customer)
            .Include(x => x.CustomerBookTrip)
                .ThenInclude(x => x.Trip)
                    .ThenInclude(x=>x.Route)
            .FirstOrDefaultAsync();
            if (ticket == null)
            {
                dto.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                dto.Message = $"Ticket with Id {id} not found!";
            }
            dto.Ticket = ticket;
        }
        catch (Exception ex)
        {
            dto.StatusCode = System.Net.HttpStatusCode.InternalServerError;
            dto.Message = ex.Message;
        }
        return dto;
    }

    public async Task<TicketCreateOrUpdateDTO> Update(Ticket ticket)
    {
        TicketCreateOrUpdateDTO dto = new();
        try
        {
            var currentState = context.Entry(ticket).State;
            context.Entry(ticket).State = EntityState.Modified;
            // context.Entry(user).Property("RowVersion").OriginalValue = user.RowVersion;
            context.Update(ticket);
            await context.SaveChangesAsync();

        }
        catch (DbUpdateConcurrencyException ex)
        {
            dto.StatusCode = HttpStatusCode.Conflict;

            var exceptionEntry = ex.Entries.Single();
            var clientValues = (Models.Ticket)exceptionEntry.Entity;
            var databaseEntry = exceptionEntry.GetDatabaseValues();
            if (databaseEntry == null)
            {
                dto.Message =
                    "Unable to save changes. The User was deleted by another user.";
            }
            else
            {
                var databaseValues = (Models.Ticket)databaseEntry.ToObject();

                var cbt = await customerBookTripsDal.GetCustomerBookTripById(databaseValues.CustomerBookTripId);
                var customer = cbt.CustomerBookTrip.Customer;
                var trip = cbt.CustomerBookTrip.Trip;
                var generalParam = databaseValues.GeneralParamId.HasValue?await generalParamDal.GetGeneralParamById(databaseValues.GeneralParamId.GetValueOrDefault()):null;
                if (databaseValues.CustomerBookTripId != clientValues.CustomerBookTripId)
                {
                    dto.Message = $"Customer and Trip - Current value: {customer.Name} currently books {trip.Route?.RouteDescription} - {trip.RegistrationNumber}";
                }

                if (databaseValues.Price != clientValues.Price)
                {
                    dto.Message = $"Price - Current value: {databaseValues.Price}";
                }

                if (databaseValues.SellerCode != clientValues.SellerCode)
                {
                    dto.Message = $"Seller Code - Current value: {databaseValues.SellerCode}";
                }

                if (databaseValues.GeneralParamId != clientValues.GeneralParamId)
                {
                    dto.Message = $"General Param Id - Current value: {generalParam?.GeneralParam?.ParamDescription}";
                }

                dto.Message += "\nThe record you attempted to edit "
                        + "was modified by another user after you got the original value. The "
                        + "edit operation was canceled and the current values in the database "
                        + "have been displayed. If you still want to edit this record, click "
                        + "the Save button again. Otherwise click the Back to List hyperlink.";
                ticket.RowVersion = (byte[])databaseValues.RowVersion;
            }
        }
        catch (Exception ex)
        {
            dto.Message = ex.Message;
            dto.StatusCode = System.Net.HttpStatusCode.InternalServerError;
        }
        finally
        {
            dto.Ticket = ticket;

        }

        return dto;
    }
}