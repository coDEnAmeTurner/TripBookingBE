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

    private readonly ICustomerBookTripsDal bookingDAL;
    private readonly ICustomerReviewTripsDal reviewDAL;

    public UsersDal(TripBookingContext context, ICustomerBookTripsDal bookingDAL, ICustomerReviewTripsDal reviewDAL)
    {
        this.context = context;
        this.bookingDAL = bookingDAL;
        this.reviewDAL = reviewDAL;
    }

    public async Task<UserCreateOrUpdateDTO> Create(User user)
    {
        UserCreateOrUpdateDTO dto = new();

        try
        {
            context.Add(user);
            await context.SaveChangesAsync();

            dto.User = user;
        }
        catch (Exception ex)
        {
            dto.StatusCode = System.Net.HttpStatusCode.InternalServerError;
            dto.Message = ex.Message;
        }

        return dto;
    }

    public async Task<UserDeleteDTO> DeleteUser(long id)
    {
        UserDeleteDTO dto = new();
        using (TransactionScope scope = new TransactionScope())
        {
            try
            {
                var bookingDTO = await bookingDAL.DeleteCustomerBookTripsByUser(id);
                if (bookingDTO.StatusCode != HttpStatusCode.NoContent)
                {
                    dto.StatusCode = bookingDTO.StatusCode;
                    dto.Message = bookingDTO.Message;
                    throw new Exception(dto.Message);
                }

                var reviewDTO = await reviewDAL.DeleteCustomerReviewTripsByUser(id);
                if (reviewDTO.StatusCode != HttpStatusCode.NoContent)
                {
                    dto.StatusCode = reviewDTO.StatusCode;
                    dto.Message = reviewDTO.Message;
                    throw new Exception(dto.Message);
                }

                var inst = await context.Users.FindAsync(id);
                if (inst == null)
                {
                    dto.StatusCode = System.Net.HttpStatusCode.NotFound;
                    dto.Message = $"User with Id {id} not found!";
                    throw new Exception(dto.Message);
                }

                context.Users.Remove(inst);
                await context.SaveChangesAsync();

                dto.User = inst;
            }
            catch (Exception ex)
            {
                if (dto.StatusCode == HttpStatusCode.NoContent)
                {
                    dto.StatusCode = HttpStatusCode.InternalServerError;
                    dto.Message = ex.Message;
                }
            }
            scope.Complete();
        }
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

    public UserGetUsersDTO GetUsers(string name, string type, string sellerCode, string email)
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

            users = users.OrderByDescending(u => u.Id);

            dto.Users = users;
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
            user.Password = user.NewPassword == null ? user.Password : user.NewPassword;
            context.Update(user);
            await context.SaveChangesAsync();

            dto.User = user;
        }
        catch (Exception ex)
        {
            dto.Message = ex.Message;
            dto.StatusCode = System.Net.HttpStatusCode.InternalServerError;
        }

        return dto;
    }
}