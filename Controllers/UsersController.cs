using System.Net;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
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
        var dto = usersService.GetUsers(name, type, sellerCode, email);
        if (dto.StatusCode != HttpStatusCode.OK)
        {
            ViewData["statusCode"] = dto.StatusCode;
            ViewData["errorMessage"] = dto.Message;
            return View();
        }

        return View(dto.Users);
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
        
        return View("Index",model: usersService.GetUsers().Users);
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
                if (targetUser.StatusCode != HttpStatusCode.Created)
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

