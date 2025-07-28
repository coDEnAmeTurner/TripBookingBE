using System.Net;
using System.Transactions;
using AspNetCoreGeneratedDocument;
using Azure.Identity;
using Microsoft.EntityFrameworkCore;
using TripBookingBE.Dal.DalInterfaces;
using TripBookingBE.Data;
using TripBookingBE.DTO.UserDTO;
using TripBookingBE.Models;

namespace TripBookingBE.Dal.DalImplementations;

public class UsersDal : IUsersDal
{
    private readonly TripBookingContext context;



    public UsersDal(TripBookingContext context)
    {
        this.context = context;
    }

    public async Task<UserCreateOrUpdateDTO> Create(User user)
    {
        UserCreateOrUpdateDTO dto = new();

        try
        {
            context.Add(user);
            await context.SaveChangesAsync();

        }
        catch (Exception ex)
        {
            dto.StatusCode = System.Net.HttpStatusCode.InternalServerError;
            dto.Message = ex.Message;
        }
        finally
        {
            dto.User = user;

        }

        return dto;
    }

    public async Task<UserDeleteDTO> DeleteUser(long id)
    {
        UserDeleteDTO dto = new();

        var inst = await context.Users.FindAsync(id);
        if (inst == null)
        {
            dto.StatusCode = System.Net.HttpStatusCode.NotFound;
            dto.Message += $"\nUser with Id {id} not found!";
        }

        context.Users.Remove(inst);
        await context.SaveChangesAsync();

        dto.User = inst;
        return dto;
    }

    public async Task<UserGetByIdDTO> GetUserById(long id)
    {
        UserGetByIdDTO dto = new();
        try
        {
            var user = await context.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (user == null)
            {
                dto.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                dto.Message = $"User with Id {id} not found!";
            }
            dto.User = user;
        }
        catch (Exception ex)
        {
            dto.StatusCode = System.Net.HttpStatusCode.InternalServerError;
            dto.Message = ex.Message;
        }
        return dto;
    }

    public async Task<UserGetUsersDTO> GetUsers(string name, string type, string sellerCode, string email)
    {

        UserGetUsersDTO dto = new();
        try
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

            var resultusers = await users.Include(u => u.Trips).ThenInclude(t => t.Route).OrderByDescending(u => u.Id).ToListAsync();

            dto.Users = resultusers;
        }
        catch (Exception ex)
        {
            dto.StatusCode = System.Net.HttpStatusCode.InternalServerError;
            dto.Message = ex.Message;
        }

        return dto;
    }

    public async Task<UserCreateOrUpdateDTO> Update(User user)
    {
        UserCreateOrUpdateDTO dto = new();
        try
        {
            var currentState = context.Entry(user).State;
            context.Entry(user).State = EntityState.Modified;
            // context.Entry(user).Property("RowVersion").OriginalValue = user.RowVersion;
            user.Password = user.NewPassword == null ? user.Password : user.NewPassword;
            context.Update(user);
            await context.SaveChangesAsync();

        }
        catch (DbUpdateConcurrencyException ex)
        {
            dto.StatusCode = HttpStatusCode.Conflict;

            var exceptionEntry = ex.Entries.Single();
            var clientValues = (User)exceptionEntry.Entity;
            var databaseEntry = exceptionEntry.GetDatabaseValues();
            if (databaseEntry == null)
            {
                dto.Message =
                    "Unable to save changes. The User was deleted by another user.";
            }
            else
            {
                var databaseValues = (User)databaseEntry.ToObject();

                if (databaseValues.Name != clientValues.Name)
                {
                    dto.Message = $"Name - Current value: {databaseValues.Name}";
                }
                if (databaseValues.UserName != clientValues.UserName)
                {
                    dto.Message = $"Username - Current value: {databaseValues.UserName}";
                }
                if (databaseValues.FirstName != clientValues.FirstName)
                {
                    dto.Message = $"FirstName - Current value: {databaseValues.FirstName}";
                }
                if (databaseValues.LastName != clientValues.LastName)
                {
                    dto.Message = $"LastName - Current value: {databaseValues.LastName}";
                }
                if (databaseValues.Email != clientValues.Email)
                {
                    dto.Message = $"Email - Current value: {databaseValues.Email}";
                }
                if (databaseValues.Avatar != clientValues.Avatar)
                {
                    dto.Message = $"Avatar - Current value: {databaseValues.Avatar}";
                }
                if (databaseValues.Phone != clientValues.Phone)
                {
                    dto.Message = $"Phone - Current value: {databaseValues.Phone}";
                }
                if (databaseValues.Type != clientValues.Type)
                {
                    dto.Message = $"Type - Current value: {databaseValues.Type}";
                }
                if (databaseValues.SellerCode != clientValues.SellerCode)
                {
                    dto.Message = $"SellerCode - Current value: {databaseValues.SellerCode}";
                }

                dto.Message += "\nThe record you attempted to edit "
                        + "was modified by another user after you got the original value. The "
                        + "edit operation was canceled and the current values in the database "
                        + "have been displayed. If you still want to edit this record, click "
                        + "the Save button again. Otherwise click the Back to List hyperlink.";
                user.RowVersion = (byte[])databaseValues.RowVersion;
            }
        }
        catch (Exception ex)
        {
            dto.Message = ex.Message;
            dto.StatusCode = System.Net.HttpStatusCode.InternalServerError;
        }
        finally
        {
            dto.User = user;

        }

        return dto;
    }
}