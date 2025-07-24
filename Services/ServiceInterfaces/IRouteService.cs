using Microsoft.EntityFrameworkCore.Metadata.Internal;
using TripBookingBE.DTO.RouteDTO;

namespace TripBookingBE.Services.ServiceInterfaces;

public interface IRouteService
{
    public Task<RouteGetRoutesDTO> GetRoutes(string? description, DateTime? dateCreated);

    Task<RouteGetCreateOrUpdateModelDTO> GetCreateOrUpdateModel(long? id);

    Task<RouteCreateOrUpdateDTO> CreateOrUpdate(Models.Route route);

    Task<RouteGetByIdDTO> GetRouteById(long id);

    Task<RouteDeleteDTO> DeleteRoute(long id);


}