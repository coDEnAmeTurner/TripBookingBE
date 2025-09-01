using TripBookingBE.Data;
using TripBookingBE.Services.ServiceInterfaces;
using TripBookingBE.Services.ServiceImplementations;
using TripBookingBE.Dal.DalInterfaces;
using TripBookingBE.Dal.DalImplementations;
using TripBookingBE.security;
using Microsoft.AspNetCore.Authorization;
using TripBookingBE.Requirements.TicketOfCustomer;
using TripBookingBE.Requirements.UpdateUserDetails;
using TripBookingBE.Requirements.TicketSeller;
using Microsoft.IdentityModel.Logging;
using Microsoft.EntityFrameworkCore;
using dotenv.net;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using TripBookingBE.Commons.Configurations;
using TripBookingBE.Commons.VnPayLibrary;
using Microsoft.AspNetCore.HttpOverrides;
using SendGrid.Extensions.DependencyInjection;
using RabbitMQ.Client;

IdentityModelEventSource.ShowPII = true;

var builder = WebApplication.CreateBuilder(args);

//httpcontext injection
builder.Services.AddHttpContextAccessor();

//configs
var config = new ConfigurationBuilder()
            .SetBasePath($"{Directory.GetCurrentDirectory()}")
            .AddJsonFile("appsettings.json")
            .Build();
builder.Services.Configure<ConnectionStrings>(config.GetSection("ConnectionStrings"));
builder.Services.Configure<VnPayConfigs>(config.GetSection("VnPayConfigs"));
builder.Services.Configure<SendGridConfigs>(config.GetSection("SendGridConfigs"));
builder.Services.Configure<SendGridConfigs>(config.GetSection("SendGridConfigs"));
builder.Services.Configure<RabbitMqConfigs>(config.GetSection("RabbitMqConfigs"));

//vnpay
builder.Services.AddScoped<Utils>();
builder.Services.AddScoped<VnPayCompare>();
builder.Services.AddScoped<VnPayLibrary>();

//rpc client
builder.Services.AddSingleton<IConnectionFactory>(new ConnectionFactory()
{
    HostName = config.GetValue<string>("RabbitMqConfigs:HostName"),
    Port = config.GetValue<int>("RabbitMqConfigs:Port")
});
builder.Services.AddScoped<IRpcClient, RpcClient>();

//sendgrid client
builder.Services.AddSendGrid(options =>
        options.ApiKey = config.GetValue<string>("SendGridConfigs:ApiKey")
    );

// add rest api controller
builder.Services.AddControllersWithViews();

//db context
builder.Services.AddDbContext<TripBookingContext>(options => options.UseMySQL(config.GetConnectionString("TripBookingContext_MySQL")));

//cloudinary
DotEnv.Load(options: new DotEnvOptions(probeForEnv: true));
var str = Environment.GetEnvironmentVariable("CLOUDINARY_URL");
Cloudinary cloudinary = new Cloudinary(str);
cloudinary.Api.Secure = true;
builder.Services.AddSingleton(typeof(Cloudinary), cloudinary);

//token gen
builder.Services.AddSingleton<TokenGenerator>();

builder.Services.AddHttpContextAccessor();

//services and dals
builder.Services.AddScoped<IUsersService, UsersService>();
builder.Services.AddScoped<IUsersDal, UsersDal>();
builder.Services.AddScoped<IBookingsService, BookingsService>();
builder.Services.AddScoped<IBookingsDal, BookingsDal>();
builder.Services.AddScoped<IReviewService, ReviewsService>();
builder.Services.AddScoped<IReviewsDal, ReviewsDal>();
builder.Services.AddScoped<IRouteService, RouteService>();
builder.Services.AddScoped<IRouteDAL, RouteDAL>();
builder.Services.AddScoped<ITripService, TripService>();
builder.Services.AddScoped<ITripDAL, TripDAL>();
builder.Services.AddScoped<ITicketService, TicketService>();
builder.Services.AddScoped<ITicketDAL, TicketDAL>();
builder.Services.AddScoped<IGeneralParamService, GeneralParamService>();
builder.Services.AddScoped<IGeneralParamDal, GeneralParamDal>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ITestService, TestService>();

//session
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(1);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

//auth handler
builder.Services.AddScoped<IAuthorizationHandler, TicketOfCustomerHanlder>();
builder.Services.AddScoped<IAuthorizationHandler, TicketSellerHanlder>();
builder.Services.AddScoped<IAuthorizationHandler, UpdateUserDetailsHanlder>();

//auth      
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AllowAll", policy => policy.RequireRole("ADMIN", "SELLER", "CUSTOMER", "DRIVER"));
    options.AddPolicy("AllowAdminOnly", policy => policy.RequireRole("ADMIN"));
    options.AddPolicy("AllowAdminOrCust", policy => policy.RequireRole("ADMIN", "CUSTOMER"));
    options.AddPolicy("AllowDriverOnly", policy => policy.RequireRole("ADMIN", "DRIVER"));
    options.AddPolicy("AllowCustOnly", policy => policy.RequireRole("ADMIN", "CUSTOMER"));
    options.AddPolicy("AllowSellerOnly", policy => policy.RequireRole("ADMIN", "SELLER"));
    options.AddPolicy("AllowDriverOrTicketOwner", policy => policy.AddRequirements(new TicketOfCustomerRequirement()));
    options.AddPolicy("AllowDriverOrSeller", policy => policy.RequireRole("ADMIN", "DRIVER", "SELLER"));
    options.AddPolicy("AllowTicketSeller", policy => policy.AddRequirements(new TicketSellerRequirement()));
    options.AddPolicy("AllowUpdateUserDetails", policy => policy.AddRequirements(new UpdateUserDetailsRequirement()));
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(x =>
        {
            x.TokenValidationParameters = new TokenValidationParameters
            {

                IssuerSigningKey = new SymmetricSecurityKey("893u498423-n2u8y07134pjoigvrew0y82453jpir-e90135 kjsdfg"u8.ToArray()),
                ValidIssuer = "https://localhost:7078",
                ValidAudience = "https://localhost:7078",
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                ValidateIssuer = true,
                ValidateAudience = true
            };

            x.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    Console.WriteLine("JWT authentication failed:");
                    Console.WriteLine(context.Exception.ToString());
                    return Task.CompletedTask;
                }
            };
        }
    );


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

app.UseHttpsRedirection();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor |
    ForwardedHeaders.XForwardedProto
});

app.MapStaticAssets();

//conventional routing + session
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

//attribute routing
// app.MapControllers();


app.Run();