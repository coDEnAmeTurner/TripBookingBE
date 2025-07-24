using NuGet.Common;
using TripBookingBE.DTO.RouteDTO;

namespace  TripBookingBE.Dal.DalInterfaces;

public interface IRouteDAL
{
    public Task<RouteGetRoutesDTO> GetRoutes(string description, DateTime? dateCreated);
}