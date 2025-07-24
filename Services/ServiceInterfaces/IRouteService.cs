using TripBookingBE.DTO.RouteDTO;

namespace TripBookingBE.Services.ServiceInterfaces;

public interface IRouteService
{
    public Task<RouteGetRoutesDTO> GetRoutes(string description, DateTime? dateCreated);
}