namespace TripBookingBE.Web.Middleware.Startup;

public class HelloTalkerFilter : IStartupFilter
{
    public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
    {
        return builder =>
        {
            builder.UseMiddleware<HelloTalkerMiddleware>();
            next(builder);
        };
    }
}