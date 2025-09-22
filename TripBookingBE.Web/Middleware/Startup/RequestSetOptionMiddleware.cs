using System.Net;

namespace TripBookingBE.Web.Middleware.Startup;

public class RequestSetOptionsMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestSetOptionsMiddleware> logger;

    public RequestSetOptionsMiddleware(RequestDelegate next, ILogger<RequestSetOptionsMiddleware> logger)
    {
        _next = next;
        this.logger = logger;
    }

    // Test with https://localhost:5001/Privacy/?option=Hello
    public async Task Invoke(HttpContext httpContext)
    {
        var option = httpContext.Request.Query["options"];

        if (!string.IsNullOrWhiteSpace(option))
        {
            httpContext.Items["options"] = "Hello :)";
        }

        logger.LogInformation($"Options: {option}");

        await _next(httpContext);
    }
}