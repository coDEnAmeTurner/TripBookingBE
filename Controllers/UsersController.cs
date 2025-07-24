using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using TripBookingBE.DTO.UserDTO;
using TripBookingBE.Models;
using TripBookingBE.Pagination;
using TripBookingBE.Services.ServiceInterfaces;

namespace TripBookingBE.Controllers;

public class UsersController : Controller
{
    private IUsersService usersService;

    public UsersController(IUsersService usersService)
    {
        this.usersService = usersService;
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

