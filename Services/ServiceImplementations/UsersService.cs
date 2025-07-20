using System.IO.MemoryMappedFiles;
using System.Net;
using System.Threading.Tasks;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.VisualBasic;
using TripBookingBE.Dal.DalInterfaces;
using TripBookingBE.Data;
using TripBookingBE.DTO.UserDTO;
using TripBookingBE.Models;
using TripBookingBE.Services.ServiceInterfaces;

namespace TripBookingBE.Services.ServiceImplementations;

public class UsersService : IUsersService
{
    private readonly IUsersDal usersDAL;
    private readonly Cloudinary cloudinary;

    public UsersService(IUsersDal dal, Cloudinary cloudinary)
    {
        this.usersDAL = dal;
        this.cloudinary = cloudinary;
    }

    public async Task<UserCreateOrUpdateDTO> CreateOrUpdate(User user)
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
                        dto.StatusCode = uploadResult.StatusCode;
                        dto.Message = uploadResult.Error.Message;
                        return dto;
                    }

                    user.Avatar = uploadResult.SecureUrl.AbsoluteUri;
                }
                catch (Exception ex)
                {
                    dto.StatusCode = HttpStatusCode.InternalServerError;
                    dto.Message = ex.Message;
                    return dto;
                }
            }
        }

        UserCreateOrUpdateDTO dtoDAL = new();
        if (user.Id == 0)
        {
            if (!user.Type.Equals("SELLER"))
                user.SellerCode = null;

            dtoDAL = await usersDAL.Create(user);
        }
        else
        {
            //check for password, with jwt configured

            dtoDAL = await usersDAL.Update(user);
        }
        dto.User = dtoDAL.User;
        if (dtoDAL.StatusCode != HttpStatusCode.OK)
        {
            dto.StatusCode = dtoDAL.StatusCode;
            dto.Message = dtoDAL.Message;
        }
        return dto;
    }

    public async Task<UserDeleteDTO> DeleteUser(long id)
    {
        UserDeleteDTO dto = await usersDAL.DeleteUser(id);
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
            dto.StatusCode = dtoDAL.StatusCode;
            dto.Message = dtoDAL.Message;
        }

        return dto;
    }

    public async Task<UserGetByIdDTO> GetUserById(long id)
    {
        var dto = await usersDAL.GetUserById(id);
        return dto;
    }

    public UserGetUsersDTO GetUsers(string name, string type, string sellerCode, string email)
    {
        return usersDAL.GetUsers(name, type, sellerCode, email);
    }
}