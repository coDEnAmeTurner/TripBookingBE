using System.IO.MemoryMappedFiles;
using System.Net;
using System.Threading.Tasks;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.VisualBasic;
using TripBookingBE.Dal.DalInterfaces;
using TripBookingBE.Data;
using TripBookingBE.DTO;
using TripBookingBE.Models;
using TripBookingBE.Services.ServiceInterfaces;

namespace TripBookingBE.Services.ServiceImplementations;

public class UsersServiceImpl : IUsersService
{
    private readonly IUsersDal dal;

    private readonly Cloudinary cloudinary;

    public UsersServiceImpl(IUsersDal dal, Cloudinary cloudinary)
    {
        this.dal = dal;
        this.cloudinary = cloudinary;
    }

    public async Task<UserCreateOrUpdateDTO> CreateOrUpdate(long? id, string Password, string NewPassword, string ConfirmPassword, string UserName, string FirstName, string LastName, string Email, bool Active, string Name, string Phone, string Type, string SellerCode, IFormFile file)
    {
        UserCreateOrUpdateDTO dto = new();

        string Avatar = string.Empty;
        using (var memoryStream = new MemoryStream())
        {
            try
            {
                file.CopyTo(memoryStream);
                var bytes = memoryStream.ToArray();
                Stream stream = new MemoryStream(bytes);
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription()
                    {
                        FileName = file.FileName,
                        FileSize = file.Length,
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

                Avatar = uploadResult.SecureUrl.AbsoluteUri;
            }
            catch (Exception ex)
            {
                dto.StatusCode = HttpStatusCode.InternalServerError;
                dto.Message = ex.Message;
                return dto;
            }
        }

        UserCreateOrUpdateDTO dtoDAL = new();
        if (id == 0)
        {
            if (!Type.Equals("SELLER"))
                SellerCode = null;

            dtoDAL = await dal.Create(Password, UserName, FirstName, LastName, Email, Active, Name, Phone, Type, SellerCode, Avatar);
        }
        else
        {
            //check for password, with jwt configured
            //up file cloudinary
            dtoDAL = await dal.Update(id.GetValueOrDefault(), Password, UserName, FirstName, LastName, Email, Active, Name, Phone, Type, SellerCode, Avatar);
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
        

        return await dal.DeleteUser(id);
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
        var dto = await dal.GetUserById(id);
        return dto;
    }

    public UserGetUsersDTO GetUsers(string name, string type, string sellerCode, string email)
    {
        return dal.GetUsers(name, type, sellerCode, email);
    }
}