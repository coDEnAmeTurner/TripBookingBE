namespace Microsoft.Extensions.DependencyInjection
{
    public static class MyConfigServiceCollectionExtensions
    {
        public static IServiceCollection AddMyDependencyGroup(
             this IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddScoped<IMyDependency, MyDependency2>();

            return services;
        }
    }
}