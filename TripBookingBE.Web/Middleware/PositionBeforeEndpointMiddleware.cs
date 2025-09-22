namespace TripBookingBE.Web.Middleware.Startup;

public class PositionBeforeEndpointMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<PositionBeforeEndpointMiddleware> logger;

    public PositionBeforeEndpointMiddleware(RequestDelegate next, ILogger<PositionBeforeEndpointMiddleware> logger)
    {
        _next = next;
        this.logger = logger;
    }

    // Test with https://localhost:5001/Privacy/?option=Hello
    public async Task Invoke(HttpContext httpContext)
    {
        logger.LogInformation($"This one is somewhere before endpoint: {nameof(PositionBeforeEndpointMiddleware)}");
        var des = httpContext.Request.Query["Description"];
        logger.LogInformation($"Description: {des}");
        if (des == "Route A")
        {
            throw new Exception("No Route A");
        }
        await _next(httpContext);
    }
}

public static class PositionBeforeEndpointMiddlewareExtensions
{
    public static IApplicationBuilder UsePositionBeforeEndpoint(
        this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<PositionBeforeEndpointMiddleware>();
    }
}