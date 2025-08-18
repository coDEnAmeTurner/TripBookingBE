using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using TripBookingBE.DTO.UserDTO;
using TripBookingBE.Models;
using TripBookingBE.Pagination;
using TripBookingBE.security;
using TripBookingBE.Services.ServiceInterfaces;

namespace TripBookingBE.Controllers;

public class UsersController : Controller
{
    private IUsersService usersService;

    public UsersController(IUsersService usersService
    )
    {
        this.usersService = usersService;
    }
    public async Task<IActionResult> Login()
    {
        return View();
    }

    public async Task<IActionResult> Logout()
    {
        HttpContext.Session.Clear();

        return RedirectToAction(nameof(Login));
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var user = await usersService.LogUserInMVC(username: request.Email, password: request.Password);
        if (user == null || user.User == null)
        {
            ViewData["errorMessage"] = user.Message;
            ViewData["statusCode"] = user.RespCode;
            return View();
        }
        var obj = user.User;
        HttpContext.Session.SetString("UserName", obj.UserName);
        HttpContext.Session.SetString("Phone", obj.Phone);
        HttpContext.Session.SetString("Email", obj.Email);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Index(string name, string type, string sellerCode, string email, int? pageNumber)
    {
        var dto = await usersService.GetUsers(name, type, sellerCode, email);

        if (dto.RespCode != HttpStatusCode.OK)
        {
            ViewData["statusCode"] = dto.RespCode;
            ViewData["errorMessage"] = dto.Message;
            return View();
        }

        int pageSize = 3;
        return View(await PaginatedList<User>.CreateAsync(dto.Users, pageNumber ?? 1, pageSize));
    }

    public async Task<IActionResult> Details(long? id)
    {
        var dto = await usersService.GetUserById(id.GetValueOrDefault());
        if (dto.RespCode != HttpStatusCode.OK || dto.User == null)
        {
            ViewData["statusCode"] = dto.RespCode;
            ViewData["errorMessage"] = dto.Message;
            return View("Index");
        }

        return View(dto.User);
    }

    public async Task<IActionResult> Delete(long id)
    {
        var dto = await usersService.DeleteUser(id);
        if (dto.RespCode != HttpStatusCode.NoContent)
        {
            ViewData["statusCode"] = dto.RespCode;
            ViewData["errorMessage"] = dto.Message;
        }

        return View("Index", model: (await usersService.GetUsers()).Users);
    }

    public async Task<IActionResult> CreateOrUpdate(long? id)
    {
        var dto = await usersService.GetCreateOrUpdateModel(id);
        if (dto.RespCode != HttpStatusCode.OK)
        {
            ViewData["statusCode"] = dto.RespCode;
            ViewData["errorMessage"] = dto.Message;
            return View();
        }
        return View(dto.User);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateOrUpdate([Bind("Id,Password,PasswordHash,NewPassword,ConfirmPassword,UserName,FirstName,LastName,Email,Active,Name,Phone,Type,SellerCode,File,DateCreated,DateModified,Avatar,RowVersion")] User user)
    {
        if (ModelState.IsValid)
        {
            UserCreateOrUpdateDTO targetUser = new() { User = user };

            if (user.Id == 0 && user.File == null)
            {
                ViewData["avatarError"] = "Avatar is missing!";
                return View(user);
            }

            targetUser = await usersService.CreateOrUpdate(user);
            if (targetUser.RespCode != HttpStatusCode.Created)
            {
                ViewData["statusCode"] = targetUser.RespCode;
                ViewData["errorMessage"] = targetUser.Message;
                if (targetUser.RespCode == HttpStatusCode.Conflict)
                    ModelState.Remove("RowVersion");
                return View(targetUser.User);
            }
            return RedirectToAction(nameof(Index));
        }
        return View(user);
    }
}

