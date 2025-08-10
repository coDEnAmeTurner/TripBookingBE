using System.Net;
using System.Transactions;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Identity;
using TripBookingBE.Dal.DalInterfaces;
using TripBookingBE.DTO.UserDTO;
using TripBookingBE.Models;
using TripBookingBE.security;
using TripBookingBE.Services.ServiceInterfaces;
using BCrypt.Net;

namespace TripBookingBE.Services.ServiceImplementations;

public class UsersService : IUsersService
{
    private readonly IUsersDal usersDAL;
    private readonly IBookingsDal bookingDAL;
    private readonly IReviewsDal reviewDAL;
    private readonly Cloudinary cloudinary;
    private readonly TokenGenerator generator;

    private readonly IPasswordHasher passwordHasher;

    public UsersService(IUsersDal dal, Cloudinary cloudinary, IBookingsDal bookingDAL, IReviewsDal reviewDAL, TokenGenerator generator, IPasswordHasher passwordHasher)
    {
        this.usersDAL = dal;
        this.cloudinary = cloudinary;
        this.bookingDAL = bookingDAL;
        this.reviewDAL = reviewDAL;
        this.generator = generator;
        this.passwordHasher = passwordHasher;
    }

    public async Task<UserCreateOrUpdateDTO> CreateOrUpdate(User user, bool api = false)
    {
        UserCreateOrUpdateDTO dto = new();

        if (user.File != null)
        {
            using (var memoryStream = new MemoryStream())
            {
                try
                {
                    user.File.CopyTo(memoryStream);
                    var bytes = memoryStream.ToArray();
                    Stream stream = new MemoryStream(bytes);
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription()
                        {
                            FileName = user.File.FileName,
                            FileSize = user.File.Length,
                            Stream = stream
                        },
                        UseFilename = true
                    };
                    var uploadResult = cloudinary.Upload(uploadParams);
                    if (uploadResult.StatusCode != HttpStatusCode.OK)
                    {
                        dto.RespCode = uploadResult.StatusCode;
                        dto.Message = uploadResult.Error.Message;
                        return dto;
                    }

                    user.Avatar = uploadResult.SecureUrl.AbsoluteUri;
                }
                catch (Exception ex)
                {
                    dto.RespCode = HttpStatusCode.InternalServerError;
                    dto.Message = ex.Message;
                    return dto;
                }
            }
        }

        UserCreateOrUpdateDTO dtoDAL = new();
        dtoDAL.User = user;
        if (user.Id == 0)
        {
            if (!user.Type.Equals("SELLER"))
                user.SellerCode = null;

            if (string.IsNullOrEmpty(user.Password))
            {
                dtoDAL.RespCode = HttpStatusCode.BadRequest;
                dtoDAL.Message = "Password is missing.";
                return dtoDAL;
            }

            user.PasswordHash = passwordHasher.Hash(user.Password);
            dtoDAL = await usersDAL.Create(user);
        }
        else
        {
            if (api == true || (!string.IsNullOrEmpty(user.NewPassword) && user.NewPassword.Equals(user.ConfirmPassword)))
            {
                if (api == true || passwordHasher.Verify(user.PasswordHash, user.Password))
                    user.PasswordHash = passwordHasher.Hash(user.NewPassword);
                else
                {
                    dtoDAL.RespCode = HttpStatusCode.BadRequest;
                    dtoDAL.Message = "Current Password is incorrect.";
                    return dtoDAL;
                }
            }

            //check for password, with jwt configured
            dtoDAL = await usersDAL.Update(user);
        }
        dto.User = dtoDAL.User;
        if (dtoDAL.RespCode != HttpStatusCode.OK)
        {
            dto.RespCode = dtoDAL.RespCode;
            dto.Message = dtoDAL.Message;
        }
        return dto;
    }

    public async Task<UserDeleteDTO> DeleteUser(long id)
    {
        UserDeleteDTO dto = new();
        using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            try
            {
                var bookingDTO = await bookingDAL.DeleteBookingsByUser(id);
                if (bookingDTO.RespCode != HttpStatusCode.NoContent)
                {
                    dto.RespCode = bookingDTO.RespCode;
                    dto.Message = bookingDTO.Message;
                }

                var reviewDTO = await reviewDAL.DeleteReviewsByUser(id);
                if (reviewDTO.RespCode != HttpStatusCode.NoContent)
                {
                    dto.RespCode = reviewDTO.RespCode;
                    dto.Message += $"\n{reviewDTO.Message}";
                }

                var userDTO = await usersDAL.DeleteUser(id);
                if (userDTO.RespCode != HttpStatusCode.NoContent)
                {
                    dto.RespCode = userDTO.RespCode;
                    dto.Message += $"\n{userDTO.Message}";
                }

                scope.Complete();
            }
            catch (Exception ex)
            {
                if (dto.RespCode != HttpStatusCode.NoContent)
                {
                    dto.RespCode = HttpStatusCode.InternalServerError;
                    dto.Message = ex.Message;
                }
            }

        }
        return dto;
    }

    public async Task<UserGetCreateOrUpdateModelDTO> GetCreateOrUpdateModel(long? id)
    {
        UserGetCreateOrUpdateModelDTO dto = new();
        if (id == null)
            dto.User = new Models.User();
        else
        {
            var dtoDAL = await GetUserById(id.GetValueOrDefault());
            dto.User = dtoDAL.User;
            dto.RespCode = dtoDAL.RespCode;
            dto.Message = dtoDAL.Message;
        }

        return dto;
    }

    public async Task<UserGetByIdDTO> GetUserById(long id)
    {
        var dto = await usersDAL.GetUserById(id);
        return dto;
    }

    public async Task<UserGetUsersDTO> GetUsers(string name = null, string type = null, string sellerCode = null, string email = null, string username = null)
    {
        var dto = await usersDAL.GetUsers(name, type, sellerCode, email, username);

        return dto;
    }

    public async Task<UserHashDTO> Hash(string username, string password)
    {
        var dto = await GetUsers(username: username);
        var user = dto.Users.FirstOrDefault();
        user.NewPassword = password;

        var rslt = await CreateOrUpdate(user, true);

        return new UserHashDTO()
        {
            Hash = rslt.User.PasswordHash,
            User = user
        };
    }

    public async Task<UserLoginDTO> LogUserIn(string username, string password)
    {
        UserGetUsersDTO dto = null;
        if (!String.IsNullOrEmpty(username))
        {
            dto = await GetUsers(username: username);
        }

        UserLoginDTO servicedto = new();
        if (dto == null || dto.Users == null || dto.Users.Count == 0)
        {
            servicedto.RespCode = (int)HttpStatusCode.NotFound;
            servicedto.Message = "Login Failed! Check your username.";
            return servicedto;
        }

        var user = dto.Users.FirstOrDefault();
        var dbpass = user.PasswordHash;

        var result = passwordHasher.Verify(dbpass, password);
        if (!result)
        {                   
            servicedto.RespCode = (int)HttpStatusCode.Unauthorized;
            servicedto.Message = "Login Failed! Check your password.";
            return servicedto;
        }

        servicedto.AccessToken = generator.GenerateToken(user.Id, user.UserName, user.Phone, user.Email);

        return servicedto;
    }

    public async Task<UserLoginMVCDTO> LogUserInMVC(string username, string password)
    {
        UserGetUsersDTO dto = null;
        if (!String.IsNullOrEmpty(username))
        {
            dto = await GetUsers(username: username);
        }

        UserLoginMVCDTO servicedto = new();
        if (dto == null || dto.Users == null || dto.Users.Count == 0)
        {
            servicedto.RespCode = (int)HttpStatusCode.NotFound;
            servicedto.Message = "Login Failed! Check your username.";
            return servicedto;
        }

        var user = dto.Users.FirstOrDefault();
        var dbpass = user.PasswordHash;

        var result = passwordHasher.Verify(dbpass, password);
        if (!result)
        {
            servicedto.RespCode = (int)HttpStatusCode.Unauthorized;
            servicedto.Message = "Login Failed! Check your password.";
            return servicedto;
        }

        servicedto.User = user;

        return servicedto;
    }
}