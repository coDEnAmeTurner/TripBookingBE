using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.JsonWebTokens;
using TripBookingBE.Services.ServiceInterfaces;

namespace TripBookingBE.Requirements.TicketOfCustomer;

public class TicketOfCustomerHanlder : AuthorizationHandler<TicketOfCustomerRequirement>
{
    private readonly ITicketService ticketService;

    public TicketOfCustomerHanlder(ITicketService ticketService)
    {
        this.ticketService = ticketService;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context, TicketOfCustomerRequirement requirement)
    {
        var identity = context.User.Identity as ClaimsIdentity;
        if (identity == null)
            return;
        if (identity.FindFirst(x => x.Type   == ClaimTypes.Role).Value == "DRIVER")
        { context.Succeed(requirement); return; }
        if (identity.FindFirst(x => x.Type == ClaimTypes.Role).Value == "ADMIN")
        { context.Succeed(requirement); return; }

        if (context.Resource is HttpContext mvcContext)
        {
            var userId = long.Parse(identity.FindAll(x => x.Type == ClaimTypes.NameIdentifier).ElementAt(1).Value);
            var ticketId = long.Parse(mvcContext.Request.RouteValues["id"].ToString());

            var dto = await ticketService.CheckTicketOwner(ticketId, userId);
            if (dto.IsOwner == true)
                context.Succeed(requirement);
        }

        return;
    }
}