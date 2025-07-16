using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;
using TripBookingBE.Data;

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
}

