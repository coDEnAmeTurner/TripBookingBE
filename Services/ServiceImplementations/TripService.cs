using System.Net;
using System.Transactions;
using Microsoft.Extensions.Logging.Abstractions;
using TripBookingBE.Dal.DalInterfaces;
using TripBookingBE.DTO.RouteDTO;
using TripBookingBE.Services.ServiceInterfaces;

namespace TripBookingBE.Services.ServiceImplementations;

public class  : IRouteService
{
    private readonly IRouteDAL routeDAL;

    public RouteService(IRouteDAL routeDAL)
    {
        this.routeDAL = routeDAL;
    }

    public async Task<RouteGetRoutesDTO> GetRoutes(string? description, DateTime? dateCreated)
    {
        var dto = await routeDAL.GetRoutes(description, dateCreated);
        return dto;
    }
}