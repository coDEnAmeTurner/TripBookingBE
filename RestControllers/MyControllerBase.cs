
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TripBookingBE.RestControllers;
        
[Authorize(Policy = "AllowAll")]
[ApiController]
public class MyControllerBase : ControllerBase
{

}