using System.Net;
using System.Transactions;
using Microsoft.Extensions.Logging.Abstractions;
using TripBookingBE.Dal.DalInterfaces;
using TripBookingBE.DTO.RouteDTO;
using TripBookingBE.Services.ServiceInterfaces;

namespace TripBookingBE.Services.ServiceImplementations;

public class RouteService : IRouteService
{
    private readonly IRouteDAL routeDAL;

    public RouteService(IRouteDAL routeDAL)
    {
        this.routeDAL = routeDAL;
    }

    public async Task<RouteCreateOrUpdateDTO> CreateOrUpdate(Models.Route route)
    {
        RouteCreateOrUpdateDTO dto = new();

        if (route.Id == 0)
        {

            dto = await routeDAL.Create(route);
        }
        else
        {
            //check for password, with jwt configured

            dto = await routeDAL.Update(route);
        }
        return dto;
    }

    public async Task<RouteDeleteDTO> DeleteRoute(long id)
    {
        RouteDeleteDTO dto = new();
        using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            try
            {

                var routeDTO = await routeDAL.DeleteRoute(id);
                if (routeDTO.StatusCode != HttpStatusCode.NoContent)
                {
                    dto.StatusCode = routeDTO.StatusCode;
                    dto.Message += $"\n{routeDTO.Message}";
                }

                scope.Complete();
            }
            catch (Exception ex)
            {
                if (dto.StatusCode == HttpStatusCode.NoContent)
                {
                    dto.StatusCode = HttpStatusCode.InternalServerError;
                    dto.Message = ex.Message;
                }
            }

        }
        return dto;
    }

    public async Task<RouteGetCreateOrUpdateModelDTO> GetCreateOrUpdateModel(long? id)
    {
        RouteGetCreateOrUpdateModelDTO dto = new();
        if (id == null)
            dto.Route = new Models.Route();
        else
        {
            var dtoDAL = await routeDAL.GetRouteById(id.GetValueOrDefault());
            dto.Route = dtoDAL.Route;
            dto.StatusCode = dtoDAL.StatusCode;
            dto.Message = dtoDAL.Message;
        }

        return dto;
    }

    public async Task<RouteGetByIdDTO> GetRouteById(long id)
    {
        var dto = await routeDAL.GetRouteById(id);
        return dto;
    }

    public async Task<RouteGetRoutesDTO> GetRoutes(string? description, DateTime? dateCreated)
    {
        var dto = await routeDAL.GetRoutes(description, dateCreated);
        return dto;
    }
}