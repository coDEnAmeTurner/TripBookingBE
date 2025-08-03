using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using TripBookingBE.DTO.UserDTO;
using TripBookingBE.Models;
using TripBookingBE.Pagination;
using TripBookingBE.security;
using TripBookingBE.Services.ServiceInterfaces;

namespace TripBookingBE.Controllers;

public class UsersController : Controller
{
    private IUsersService usersService;
    private TokenGenerator generator;

    public UsersController(IUsersService usersService, TokenGenerator generator)
    {
        this.usersService = usersService;
        this.generator = generator;
    }

    public async Task<IActionResult> Login(LoginRequest request)
    {
        var user = await usersService.GetUsers(username: request.Email, password: request.Password);
        if (user == null || user.Users == null || user.Users.Count == 0)
        {
            return new ContentResult()
            {
                Content = "Check your username and password! Login failed!",
                StatusCode = (int)HttpStatusCode.Unauthorized
            };
        }
        var obj = user.Users.FirstOrDefault();
        UserLoginDTO dto = new()
        {
            AccessToken = generator.GenerateToken(obj.UserName, obj.Phone, obj.Email)

        };
        return new JsonResult(obj);
    }

    public async Task<IActionResult> Index(string name, string type, string sellerCode, string email, int? pageNumber)
    {
        var dto = await usersService.GetUsers(name, type, sellerCode, email);

        if (dto.StatusCode != HttpStatusCode.OK)
        {
            ViewData["statusCode"] = dto.StatusCode;
            ViewData["errorMessage"] = dto.Message;
            return View();
        }

        int pageSize = 3;
        return View(await PaginatedList<User>.CreateAsync(dto.Users, pageNumber ?? 1, pageSize));
    }

    public async Task<IActionResult> Details(long? id)
    {
        var dto = await usersService.GetUserById(id.GetValueOrDefault());
        if (dto.StatusCode != HttpStatusCode.OK || dto.User == null)
        {
            ViewData["statusCode"] = dto.StatusCode;
            ViewData["errorMessage"] = dto.Message;
            return View("Index");
        }

        return View(dto.User);
    }

    public async Task<IActionResult> Delete(long id)
    {
        var dto = await usersService.DeleteUser(id);
        if (dto.StatusCode != HttpStatusCode.NoContent)
        {
            ViewData["statusCode"] = dto.StatusCode;
            ViewData["errorMessage"] = dto.Message;
        }

        return View("Index", model: (await usersService.GetUsers()).Users);
    }

    public async Task<IActionResult> CreateOrUpdate(long? id)
    {
        var dto = await usersService.GetCreateOrUpdateModel(id);
        if (dto.StatusCode != HttpStatusCode.OK)
        {
            ViewData["statusCode"] = dto.StatusCode;
            ViewData["errorMessage"] = dto.Message;
            return View();
        }
        return View(dto.User);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateOrUpdate([Bind("Id, Password,NewPassword,ConfirmPassword,UserName,FirstName,LastName,Email,Active,Name,Phone,Type,SellerCode,File,DateCreated,DateModified,Avatar,RowVersion")] User user)
    {
        if (ModelState.IsValid)
        {
            UserCreateOrUpdateDTO targetUser = new() { User = user };
            if (String.IsNullOrEmpty(user.Password))
            {
                ViewData["passwordError"] = "Password is required!";
                return View(user);
            }

            if (user.Id == 0 && user.File == null)
            {
                ViewData["avatarError"] = "Avatar is missing!";
                return View(user);
            }

            targetUser = await usersService.CreateOrUpdate(user);
            if (targetUser.StatusCode != HttpStatusCode.Created)
            {
                ViewData["statusCode"] = targetUser.StatusCode;
                ViewData["errorMessage"] = targetUser.Message;
                if (targetUser.StatusCode == HttpStatusCode.Conflict)
                    ModelState.Remove("RowVersion");
                return View(targetUser.User);
            }
            return RedirectToAction(nameof(Index));
        }
        return View(user);
    }
}

