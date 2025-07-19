using TripBookingBE.DTO;
using TripBookingBE.Models;

namespace TripBookingBE.Services.ServiceInterfaces;

public interface IUsersService
{
    UserGetUsersDTO GetUsers(string name = null, string type= null, string sellerCode= null, string email= null);
    Task<UserGetByIdDTO> GetUserById(long id);
    Task<UserGetCreateOrUpdateModelDTO> GetCreateOrUpdateModel(long? id);
    Task<UserCreateOrUpdateDTO> CreateOrUpdate(long? id, string Password, string NewPassword, string ConfirmPassword, string UserName, string FirstName, string LastName, string Email, bool Active, string Name, string Phone, string Type, string SellerCode, IFormFile file);

    Task<UserDeleteDTO> DeleteUser(long id);
}