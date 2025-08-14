using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using TripBookingBE.Services.ServiceInterfaces;

namespace TripBookingBE.Requirements.UpdateUserDetails;

public class UpdateUserDetailsHanlder : AuthorizationHandler<UpdateUserDetailsRequirement>
{

    public UpdateUserDetailsHanlder()
    {
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context, UpdateUserDetailsRequirement requirement)
    {
        var identity = context.User.Identity as ClaimsIdentity;
        if (identity == null)
            return;
        if (identity.FindFirst(x => x.Type == ClaimTypes.Role).Value == "ADMIN")
{            context.Succeed(requirement);return; }
        if (identity.FindFirst(x => x.Type == ClaimTypes.Role).Value == "ADMIN")
{            context.Succeed(requirement);return; }

        if (context.Resource is HttpContext mvcContext)
        {
            var userId = long.Parse(identity.FindAll(x => x.Type == ClaimTypes.NameIdentifier).ElementAt(1).Value);
            var detailsId = long.Parse(mvcContext.Request.RouteValues["id"].ToString());

            if (userId == detailsId)
                context.Succeed(requirement);
        }

        return;
    }
}