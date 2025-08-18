using System.Net;
using Microsoft.EntityFrameworkCore;
using TripBookingBE.Dal.DalInterfaces;
using TripBookingBE.Data;
using TripBookingBE.DTO.RouteDTO;

namespace TripBookingBE.Dal.DalImplementations;

public class RouteDAL : IRouteDAL
{

    private readonly TripBookingContext context;

    public RouteDAL(TripBookingContext context)
    {
        this.context = context;
    }

    public async Task<RouteCreateOrUpdateDTO> Create(Models.Route route)
    {
        RouteCreateOrUpdateDTO dto = new();

        try
        {
            context.Add(route);
            await context.SaveChangesAsync();

        }
        catch (Exception ex)
        {
            dto.RespCode = System.Net.HttpStatusCode.InternalServerError;
            dto.Message = $"{ex.Message}\n{ex.InnerException.Message}";
        }
        finally
        {
            dto.Route = route;

        }

        return dto;
    }

    public async Task<RouteDeleteDTO> DeleteRoute(long id)
    {
        RouteDeleteDTO dto = new();

        var inst = await context.Routes.FindAsync(id);
        if (inst == null)
        {
            dto.RespCode = System.Net.HttpStatusCode.NotFound;
            dto.Message += $"\nRoute with Id {id} not found!";
        }

        context.Routes.Remove(inst);
        await context.SaveChangesAsync();

        dto.Route = inst;
        return dto;
    }

    public async Task<RouteGetByIdDTO> GetRouteById(long id)
    {
        RouteGetByIdDTO dto = new();
        try
        {
            var route = await context.Routes.FirstOrDefaultAsync(x => x.Id == id);
            if (route == null)
            {
                dto.RespCode = System.Net.HttpStatusCode.InternalServerError;
                dto.Message = $"User with Id {id} not found!";
            }
            dto.Route = route;
        }
        catch (Exception ex)
        {
            dto.RespCode = System.Net.HttpStatusCode.InternalServerError;
            dto.Message = ex.Message;
        }
        return dto;
    }

    public async Task<RouteGetRoutesDTO> GetRoutes(string? description, DateTime? dateCreated)
    {
        RouteGetRoutesDTO dto = new();
        try
        {

            var routes = await
                        (from route in context.Routes
                        where
                        (description == null || (route.RouteDescription != null && route.RouteDescription.ToUpper().Contains(description.ToUpper())))
                        && (dateCreated == null ||
                        (
                            (route.DateCreated != null &&
                            route.DateCreated.Value.Year == dateCreated.Value.Year
                            && route.DateCreated.Value.Month == dateCreated.Value.Month
                            && route.DateCreated.Value.Day == dateCreated.Value.Day)
                        ))
                        orderby route.Id descending
                        select route).ToListAsync();

            dto.Routes = routes;
        }
        catch (Exception ex)
        {
            dto.RespCode = System.Net.HttpStatusCode.InternalServerError;
            dto.Message = ex.Message;
        }

        return dto;
    }

    public async Task<RouteCreateOrUpdateDTO> Update(Models.Route route)
    {
        RouteCreateOrUpdateDTO dto = new();
        try
        {
            var currentState = context.Entry(route).State;
            context.Entry(route).State = EntityState.Modified;
            // context.Entry(user).Property("RowVersion").OriginalValue = user.RowVersion;
            context.Update(route);
            await context.SaveChangesAsync();

        }
        // catch (DbUpdateConcurrencyException ex)
        // {
        //     dto.RespCode = HttpStatusCode.Conflict;

        //     var exceptionEntry = ex.Entries.Single();
        //     var clientValues = (Models.Route)exceptionEntry.Entity;
        //     var databaseEntry = exceptionEntry.GetDatabaseValues();
        //     if (databaseEntry == null)
        //     {
        //         dto.Message =
        //             "Unable to save changes. The User was deleted by another user.";
        //     }
        //     else
        //     {
        //         var databaseValues = (Models.Route)databaseEntry.ToObject();

        //         if (databaseValues.RouteDescription != clientValues.RouteDescription)
        //         {
        //             dto.Message = $"RouteDescription - Current value: {databaseValues.RouteDescription}";
        //         }

        //         dto.Message += "\nThe record you attempted to edit "
        //                 + "was modified by another user after you got the original value. The "
        //                 + "edit operation was canceled and the current values in the database "
        //                 + "have been displayed. If you still want to edit this record, click "
        //                 + "the Save button again. Otherwise click the Back to List hyperlink.";
        //         route.RowVersion = (byte[])databaseValues.RowVersion;
        //     }
        // }
        catch (Exception ex)
        {
            dto.Message = ex.Message;
            dto.RespCode = System.Net.HttpStatusCode.InternalServerError;
        }
        finally
        {
            dto.Route = route;

        }

        return dto;
    }
}