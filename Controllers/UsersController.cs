using System.Net;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;
using TripBookingBE.Data;
using TripBookingBE.DTO;
using TripBookingBE.Models;
using TripBookingBE.Services.ServiceImplementations;
using TripBookingBE.Services.ServiceInterfaces;

namespace TripBookingBE.Controllers;

public class UsersController : Controller
{
    private IUsersService usersService;

    public UsersController(IUsersService usersService)
    {
        this.usersService = usersService;
    }

    public async Task<IActionResult> Index(string name, string type, string sellerCode, string email)
    {
        var users = usersService.GetUsers(name, type, sellerCode, email);

        return View(users);
    }

    public async Task<IActionResult> Details(long? id)
    {
        var user = await usersService.GetUserById(id.GetValueOrDefault());
        if (user == null)
        {
            return NotFound();
        }

        return View(user);
    }

    public async Task<IActionResult> CreateOrUpdate(long? id)
    {
        return View(await usersService.GetCreateOrUpdateModel(id));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateOrUpdate(long? id, [Bind("Password,NewPassword,ConfirmPassword,UserName,FirstName,LastName,Email,Active,Name,Phone,Type,SellerCode,File")] User user)
    {

        if (ModelState.IsValid)
        {
            UserCreateOrUpdateDTO targetUser = new() { User = user };
            try
            {
                if (String.IsNullOrEmpty(user.Password))
                {
                    ViewData["passwordError"] = "Password is required!";
                    return View(user);
                }

                targetUser = await usersService.CreateOrUpdate(id, user.Password, user.NewPassword, user.ConfirmPassword, user.UserName, user.FirstName, user.LastName, user.Email, user.Active, user.Name, user.Phone, user.Type, user.SellerCode, user.File);
                if (targetUser.StatusCode != HttpStatusCode.OK)
                {
                    ViewData["statusCode"] = targetUser.StatusCode;
                    ViewData["errorMessage"] = targetUser.Message;
                    return View(user);
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if ((await usersService.GetUserById(targetUser.User.Id)) == null)
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }

        return View(user);
    }
}

