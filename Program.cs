using Microsoft.EntityFrameworkCore;
using TripBookingBE.Data;
using CloudinaryDotNet;
using dotenv.net;
using TripBookingBE.Services.ServiceInterfaces;
using TripBookingBE.Services.ServiceImplementations;
using TripBookingBE.Dal.DalInterfaces;
using TripBookingBE.Dal.DalImplementations;

var builder = WebApplication.CreateBuilder(args);

// add rest api controller
builder.Services.AddControllersWithViews();

//db context
builder.Services.AddDbContext<TripBookingContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("TripBookingContext")));

//cloudinary
DotEnv.Load(options: new DotEnvOptions(probeForEnv: true));
Cloudinary cloudinary = new Cloudinary(Environment.GetEnvironmentVariable("CLOUDINARY_URL"));
cloudinary.Api.Secure = true;
builder.Services.AddSingleton(typeof(Cloudinary), cloudinary);

//services and dals
builder.Services.AddScoped<IUsersService,UsersServiceImpl>();
builder.Services.AddScoped<IUsersDal,UsersDalImpl>();

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