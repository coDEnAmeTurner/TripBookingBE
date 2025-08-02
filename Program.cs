using TripBookingBE.Data;
using CloudinaryDotNet;
using dotenv.net;
using TripBookingBE.Services.ServiceInterfaces;
using TripBookingBE.Services.ServiceImplementations;
using TripBookingBE.Dal.DalInterfaces;
using TripBookingBE.Dal.DalImplementations;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// add rest api controller
builder.Services.AddControllersWithViews();

//db context
builder.Services.AddDbContext<TripBookingContext>(options=>options.UseSqlServer(builder.Configuration.GetConnectionString("TripBookingContext")));

//cloudinary
DotEnv.Load(options: new DotEnvOptions(probeForEnv: true));
Cloudinary cloudinary = new Cloudinary(Environment.GetEnvironmentVariable("CLOUDINARY_URL"));
cloudinary.Api.Secure = true;
builder.Services.AddSingleton(typeof(Cloudinary), cloudinary);

//services and dals
builder.Services.AddScoped<IUsersService,UsersService>();
builder.Services.AddScoped<IUsersDal, UsersDal>();
builder.Services.AddScoped<IBookingsService,BookingsService>();
builder.Services.AddScoped<IBookingsDal, BookingsDal>();
builder.Services.AddScoped<IReviewService,ReviewsService>();
builder.Services.AddScoped<IReviewsDal,ReviewsDal>();
builder.Services.AddScoped<IRouteService, RouteService>();
builder.Services.AddScoped<IRouteDAL, RouteDAL>();
builder.Services.AddScoped<ITripService, TripService>();
builder.Services.AddScoped<ITripDAL,TripDAL>();
builder.Services.AddScoped<ITicketService, TicketService>();
builder.Services.AddScoped<ITicketDAL, TicketDAL>();
builder.Services.AddScoped<IGeneralParamService, GeneralParamService>();
builder.Services.AddScoped<IGeneralParamDal,GeneralParamDal>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

//conventional routing
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();