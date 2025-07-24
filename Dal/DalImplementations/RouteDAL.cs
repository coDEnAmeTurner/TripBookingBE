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

    public async Task<RouteGetRoutesDTO> GetRoutes(string description, DateTime? dateCreated)
    {
        RouteGetRoutesDTO dto = new();
        try
        {
            var routes = from route in context.Routes
                        select route;

            if (!String.IsNullOrEmpty(description))
            {
                routes = routes.Where(u => u.RouteDescription!.ToUpper().Contains(description.ToUpper()));
            }
            if (dateCreated != null)
            {
                routes = routes.Where(u => u.DateCreated != null && u.DateCreated.Equals(dateCreated));
            }

            var resultroutes = await routes.OrderByDescending(u => u.Id).ToListAsync();

            dto.Routes = resultroutes;
        }
        catch (Exception ex)
        {
            dto.StatusCode = System.Net.HttpStatusCode.InternalServerError;
            dto.Message = ex.Message;
        }

        return dto;
    }
}