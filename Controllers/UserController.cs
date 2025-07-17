using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;
using TripBookingBE.Data;
using TripBookingBE.Models;

namespace TripBookingBE.Controllers;

public class UserController : Controller
{
    private readonly TripBookingContext context;

    public UserController(TripBookingContext context)
    {
        this.context = context;
    }

    public async Task<IActionResult> Index(string name, string type, string sellerCode, string email)
    {
        if (context.Users == null)
            return Problem("Entity set TripBookingBE.Data.Users is null");

        var users = from user  in context.Users 
                    select user ;

        if (!String.IsNullOrEmpty(name))
        {
            users = users.Where(u => u.Name!.ToUpper().Contains(name.ToUpper())
            || u.UserName!.ToUpper().Contains(name.ToUpper())
            || u.FirstName!.ToUpper().Contains(name.ToUpper())
            || u.LastName!.ToUpper().Contains(name.ToUpper()));
        }
        if (!String.IsNullOrEmpty(type))
        {
            users = users.Where(u => u.Type.Contains(type));
        }
        if (!String.IsNullOrEmpty(sellerCode))
        {
            users = users.Where(u => u.SellerCode != null && u.SellerCode.Contains(sellerCode));
        }
        if (!String.IsNullOrEmpty(email))
        {
            users = users.Where(u => u.Email != null && u.Email.Contains(sellerCode));
        }

        users = users.OrderByDescending(u => u.Id);

        return View(users);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null || context.Users == null)
        {
            return NotFound();
        }

        var user = await context.Users.FirstOrDefaultAsync(m => m.Id == id);
        if (user == null)
        {
            return NotFound();
        }

        return View(user); ;
    }

    public async Task<IActionResult> CreateOrUpdate(long? id)
    {
        User user = new Models.User();
        if (id != null)
        {
            user = await context.Users.FindAsync(id);
            if (user == null)
                return NotFound();


        }
        return View(user);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateOrUpdate(long? id, [Bind("Password,NewPassword,ConfirmPassword,UserName,FirstName,LastName,Email,Active,Name,Phone,Type,SellerCode,Avatar")] User user)
    {

        if (ModelState.IsValid)
        {
            User targetUser = null;
            try
            {
                if (id == 0)
                {
                    if (String.IsNullOrEmpty(user.Password))
                    {
                        ViewData["passwordError"] = "Password is required!";
                        return View(user);
                    }
                    else
                    {
                        if (!user.Type.Equals("SELLER"))
                            user.SellerCode = null;
                        context.Add(user);
                    }
                }
                else
                {

                    //call service
                    //check for password, with jwt configured
                    targetUser = await context.Users.FindAsync(id.GetValueOrDefault());

                    targetUser.Password = user.Password != null ? user.Password : targetUser.Password;
                    targetUser.UserName = user.UserName;
                    targetUser.FirstName = user.FirstName;
                    targetUser.LastName = user.LastName;
                    targetUser.Email = user.Email;
                    targetUser.Active = user.Active;
                    targetUser.Name = user.Name;
                    targetUser.Phone = user.Phone;
                    targetUser.Type = user.Type;
                    user.SellerCode = user.SellerCode;
                    user.Avatar = user.Avatar;
                    //up file cloudinary
                    context.Update(targetUser);
                }
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if ((await context.Users.FindAsync(targetUser.Id)) == null)
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

