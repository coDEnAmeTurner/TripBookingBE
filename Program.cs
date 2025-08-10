using TripBookingBE.Data;
using CloudinaryDotNet;
using dotenv.net;
using TripBookingBE.Services.ServiceInterfaces;
using TripBookingBE.Services.ServiceImplementations;
using TripBookingBE.Dal.DalInterfaces;
using TripBookingBE.Dal.DalImplementations;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using TripBookingBE.security;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Logging;
using System.IdentityModel.Tokens.Jwt;

IdentityModelEventSource.ShowPII = true;

var builder = WebApplication.CreateBuilder(args);

// add rest api controller
builder.Services.AddControllersWithViews();

//db context
builder.Services.AddDbContext<TripBookingContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("TripBookingContext")));

//cloudinary
DotEnv.Load(options: new DotEnvOptions(probeForEnv: true));
Cloudinary cloudinary = new Cloudinary(Environment.GetEnvironmentVariable("CLOUDINARY_URL"));
cloudinary.Api.Secure = true;
builder.Services.AddSingleton(typeof(Cloudinary), cloudinary);

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

//session
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(1);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

//auth
builder.Services.AddAuthorization();
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

app.MapStaticAssets();

//conventional routing
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();
app.MapControllers();


app.Run();