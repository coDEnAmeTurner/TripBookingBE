using NuGet.Common;
using TripBookingBE.DTO.RouteDTO;

namespace  TripBookingBE.Dal.DalInterfaces;

public interface IRouteDAL
{
    public Task<RouteGetRoutesDTO> GetRoutes(string? description, DateTime? dateCreated);

    public Task<RouteGetByIdDTO> GetRouteById(long id);

    public Task<RouteCreateOrUpdateDTO> Create(Models.Route route);

    public    Task<RouteCreateOrUpdateDTO> Update(Models.Route route);


    
}