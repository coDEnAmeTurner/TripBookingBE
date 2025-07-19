using AspNetCoreGeneratedDocument;
using Azure.Identity;
using Microsoft.EntityFrameworkCore;
using TripBookingBE.Dal.DalInterfaces;
using TripBookingBE.Data;
using TripBookingBE.DTO;
using TripBookingBE.Models;

namespace TripBookingBE.Dal.DalImplementations;

public class UsersDalImpl : IUsersDal
{
    private readonly TripBookingContext context;

    public UsersDalImpl(TripBookingContext context)
    {
        this.context = context;
    }

    public async Task<UserCreateOrUpdateDTO> Create(string Password, string UserName, string FirstName, string LastName, string Email, bool Active, string Name, string Phone, string Type, string SellerCode, string Avatar)
    {
        UserCreateOrUpdateDTO dto = new();

        try
        {
            User u = new User()
            {
                Password = Password,
                UserName = UserName,
                FirstName = FirstName,
                LastName = LastName,
                Email = Email,
                Active = Active,
                Name = Name,
                Phone = Phone,
                Type = Type,
                SellerCode = SellerCode,
                Avatar = Avatar
            };
            context.Add(u);
            await context.SaveChangesAsync();

            dto.User = u;
        }
        catch (Exception ex)
        {
            dto.StatusCode = System.Net.HttpStatusCode.InternalServerError;
            dto.Message = ex.Message;
        }

        return dto;
    }

    public async Task<User> GetUserById(long id)
    {
        return await context.Users.FirstOrDefaultAsync(x => x.Id == id);
    }

    public IQueryable<User> GetUsers(string name, string type, string sellerCode, string email)
    {

        var users = from user in context.Users
                    select user;

        if (!String.IsNullOrEmpty(name))
        {
            users = users.Where(u => u.Name!.ToUpper().Contains(name.ToUpper())
            || u.UserName!.ToUpper().Contains(name.ToUpper())
            || u.FirstName!.ToUpper().Contains(name.ToUpper())
            || u.LastName!.ToUpper().Contains(name.ToUpper()));
        }
        if (!String.IsNullOrEmpty(type))
        {
            users = users.Where(u => u.Type.Contains(type));
        }
        if (!String.IsNullOrEmpty(sellerCode))
        {
            users = users.Where(u => u.SellerCode != null && u.SellerCode.Contains(sellerCode));
        }
        if (!String.IsNullOrEmpty(email))
        {
            users = users.Where(u => u.Email != null && u.Email.Contains(sellerCode));
        }

        users = users.OrderByDescending(u => u.Id);

        return users;
    }

    public async Task<UserCreateOrUpdateDTO> Update(long id, string Password, string UserName, string FirstName, string LastName, string Email, bool Active, string Name, string Phone, string Type, string SellerCode, string Avatar)
    {
        UserCreateOrUpdateDTO dto = new();
        try
        {

            var targetUser = await context.Users.FindAsync(id);

            targetUser.Password = Password == null ? targetUser.Password : Password;
            targetUser.UserName = UserName;
            targetUser.FirstName = FirstName;
            targetUser.LastName = LastName;
            targetUser.Email = Email;
            targetUser.Active = Active;
            targetUser.Name = Name;
            targetUser.Phone = Phone;
            targetUser.Type = Type;
            targetUser.SellerCode = SellerCode;
            targetUser.Avatar = Avatar;
            context.Update(targetUser);
            await context.SaveChangesAsync();

            dto.User = targetUser;
        }
        catch (Exception ex)
        {
            dto.Message = ex.Message;
            dto.StatusCode = System.Net.HttpStatusCode.InternalServerError;
        }

        return dto;
    }
}