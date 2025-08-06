using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using TripBookingBE.RestRequests;
using TripBookingBE.Services.ServiceInterfaces;
using HttpPostAttribute = Microsoft.AspNetCore.Mvc.HttpPostAttribute;

namespace TripBookingBE.RestControllers;

[Route("api/users")]
public class UsersRestController : MyControllerBase
{
    private readonly IUsersService usersService;

    public UsersRestController(IUsersService usersService)
    {
        this.usersService = usersService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var dto = await usersService.LogUserIn(username: request.UserName, password: request.Password);
        if (dto.RespCode != (int)HttpStatusCode.OK)
        {
            return new ContentResult()
            {
                Content = JsonSerializer.Serialize(dto),
                StatusCode = (int)HttpStatusCode.NotFound
            };
        }
        return Ok(dto);
    }

    [HttpPost("hash")]
    public async Task<IActionResult> Hash([FromBody] LoginRequest req)
    {
        var dto = await usersService.Hash(req.UserName, req.Password);
        return Ok(dto);
    }

}