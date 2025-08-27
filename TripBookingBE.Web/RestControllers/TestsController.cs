using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TripBookingBE.RestControllers;
using TripBookingBE.Services.ServiceInterfaces;
using TripBookingBE.Web.RestRequests.Test;

namespace TripBookingBE.Web.RestControllers;

[Route("api/tests")]
public class TestsController : MyControllerBase
{
    private readonly ITestService testService;

    public TestsController(ITestService testService)
    {
        this.testService = testService;
    }

    [AllowAnonymous]
    [HttpGet("sendHelloWorld")]
    public async Task<ActionResult> SendHelloWorld()
    {
        var dto = await testService.SendHelloWorld();

        if (dto.RespCode != 200)
        {
            return Problem(dto.Message);
        }

        return Ok(dto);
    }
    
    [AllowAnonymous]
    [HttpPost("sendNewTask")]
    public async Task<ActionResult> SendNewTask(TestSendNewTaskRequest request)
    {
        var dto =  await testService.SendNewTask(request.Args);

        if (dto.RespCode != 200)
        {
            return Problem(dto.Message);
        }

        return Ok(dto);
    }
}