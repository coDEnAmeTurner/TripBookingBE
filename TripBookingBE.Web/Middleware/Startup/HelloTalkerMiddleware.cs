namespace TripBookingBE.Web.Middleware;

public class HelloTalkerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<HelloTalkerMiddleware> logger;

    public HelloTalkerMiddleware(RequestDelegate next, ILogger<HelloTalkerMiddleware> logger)
    {
        _next = next;
        this.logger = logger;
    }

    // Test with https://localhost:5001/Privacy/?option=Hello
    public async Task Invoke(HttpContext httpContext)
    {
        logger.LogInformation($"Hello from: {nameof(HelloTalkerMiddleware)}");

        await _next(httpContext);
    }
}