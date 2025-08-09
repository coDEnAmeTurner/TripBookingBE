using System.ComponentModel;
using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using TripBookingBE.Models;
using TripBookingBE.RestRequests;
using TripBookingBE.Services.ServiceInterfaces;
using HttpPostAttribute = Microsoft.AspNetCore.Mvc.HttpPostAttribute;

namespace TripBookingBE.RestControllers;

[Route("api/users")]
public class UsersController : MyControllerBase
{
    private readonly IUsersService usersService;

    public UsersController(IUsersService usersService)
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
            Active = request.Active.Value,
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

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UserUpdateRequest request)
    {
        var user = await usersService.GetUserById(id);
        if (user.User == null || user.RespCode != HttpStatusCode.OK)
        {
            return NotFound(user.Message);
        }
        var obj = user.User;
        obj.File = request.File;
        obj.PasswordHash = request.PasswordHash;
        obj.Password = request.Password;
        obj.NewPassword = request.NewPassword;
        obj.ConfirmPassword = request.ConfirmPassword;
        obj.UserName = request.UserName;
        obj.FirstName = request.FirstName;
        obj.LastName = request.LastName;
        obj.Email = request.Email;
        obj.Active = request.Active.Value;
        obj.Phone = request.Phone;
        obj.Name = request.Name;
        obj.Type = request.Type;
        obj.SellerCode = request.SellerCode;
        var dto = await usersService.CreateOrUpdate(obj);
        if (dto.RespCode == HttpStatusCode.InternalServerError)
        {
            return Problem(dto.Message);
        }
        if (dto.RespCode != HttpStatusCode.Created)
        {
            return BadRequest(dto);
        }
        return Created($"/api/users/{dto.User.Id}", dto);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Details(int id)
    {
        var dto = await usersService.GetUserById(id);
        if (dto.RespCode == HttpStatusCode.OK)
        {
            return Ok(dto);
        }
        return Problem(dto.Message);

    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var dto = await usersService.DeleteUser(id);
        if (dto.RespCode == HttpStatusCode.NoContent)
        {
            return NoContent();
        }
        return Problem(dto.Message);
    }


    [HttpPost("hash")]
    public async Task<IActionResult> Hash([FromBody] LoginRequest req)
    {
        var dto = await usersService.Hash(req.UserName, req.Password);
        return Ok(dto);
    }

}