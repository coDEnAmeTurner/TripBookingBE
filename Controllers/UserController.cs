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

    public async Task<IActionResult> Index()
    {
        return View(await context.Users.ToListAsync());
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

    public async Task<IActionResult> Edit(long? id)
    {
        if (id == null)
            return NotFound();

        var user = await context.Users.FindAsync(id);
        if (user == null)
            return NotFound();

        return View(user);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(long id, [Bind("Password,NewPassword,ConfirmPassword,UserName,FirstName,LastName,Email,Active,Name,Phone,Type,SellerCode,Avatar")] User user)
    {
        var userDB = await context.Users.FindAsync(id);
        
        if (ModelState.IsValid)
        {
            try
            {
                //call service
                //check for password, with jwt configured
                userDB.Password = user.Password!=null?user.Password:userDB.Password;
                userDB.UserName = user.UserName;
                userDB.FirstName = user.FirstName;
                userDB.LastName = user.LastName;
                userDB.Email = user.Email;
                userDB.Active = user.Active;
                userDB.Name = user.Name;
                userDB.Phone = user.Phone;
                userDB.Type = user.Type;
                user.SellerCode = user.SellerCode;
                user.Avatar = user.Avatar;
                //up file cloudinary
                context.Update(userDB);
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if ((await context.Users.FindAsync(userDB.Id)) == null)
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
        return View(userDB);
    }
}

