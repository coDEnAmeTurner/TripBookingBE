using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using TripBookingBE.Models;
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
            return NotFound(dto);
        }
        return Ok(dto);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        User user = new()
        {
            File = request.File,
            Password = request.Password,
            UserName = request.UserName,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Active = request.Active,
            Phone = request.Phone,
            Name = request.Name,
            Type = request.Type,
            SellerCode = request.SellerCode
        };
        var dto = await usersService.CreateOrUpdate(user);
        if (dto.RespCode != HttpStatusCode.Created)
        {
            return BadRequest(dto);
        }
        return Created($"/api/users/{dto.User.Id}", dto);
    }



    [HttpPost("hash")]
    public async Task<IActionResult> Hash([FromBody] LoginRequest req)
    {
        var dto = await usersService.Hash(req.UserName, req.Password);
        return Ok(dto);
    }

}