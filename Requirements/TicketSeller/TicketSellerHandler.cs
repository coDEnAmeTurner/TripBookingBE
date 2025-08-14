using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using TripBookingBE.Services.ServiceInterfaces;

namespace TripBookingBE.Requirements.TicketSeller;

public class TicketSellerHanlder : AuthorizationHandler<TicketSellerRequirement>
{
    private readonly ITicketService ticketService;

    public TicketSellerHanlder(ITicketService ticketService)
    {
        this.ticketService = ticketService;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context, TicketSellerRequirement requirement)
    {
        var identity = context.User.Identity as ClaimsIdentity;
        if (identity == null)
            return;

        if (context.Resource is HttpContext mvcContext)
        {
            var sellerCode = identity.FindAll(x => x.Type == "SellerCode").ElementAt(0).Value;
            if (string.IsNullOrEmpty(sellerCode))
                return;
            if (identity.FindFirst(x => x.Type == ClaimTypes.Role).Value == "ADMIN")
            { context.Succeed(requirement); return; }

            var ticketId = long.Parse(mvcContext.Request.RouteValues["id"].ToString());

            var dto = await ticketService.CheckTicketSeller(ticketId, sellerCode);
            if (dto.IsSeller == true)
                context.Succeed(requirement);
        }

        return;
    }
}