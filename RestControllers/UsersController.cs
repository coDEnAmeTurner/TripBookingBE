using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using TripBookingBE.security;
using TripBookingBE.Services.ServiceInterfaces;
using HttpPostAttribute = Microsoft.AspNetCore.Mvc.HttpPostAttribute;
using RouteAttribute = Microsoft.AspNetCore.Components.RouteAttribute;

namespace TripBookingBE.RestControllers;

[Route("api/users")]
public class UsersController : MyControllerBase
{
    private readonly IUsersService usersService;
    private readonly TokenGenerator generator;

    public UsersController(IUsersService usersService, TokenGenerator generator)
    {
        this.usersService = usersService;
        this.generator = generator;
    }

    [HttpPost("/login")]
    public async Task<IActionResult> Login(RestRequests.LoginRequest request)
    {
        var dto = await usersService.GetUsers(username: request.UserName, password: request.Password, email: request.Email);
        if (dto == null || dto.Users == null || dto.Users.Count == 0)
        {
            return NotFound();
        }
        var user = dto.Users.FirstOrDefault();
        return new ContentResult()
        {
            Content = JsonSerializer.Serialize(generator.GenerateToken(user.UserName, user.Phone, user.Email)),
            StatusCode = (int)HttpStatusCode.OK
        };
    }
}