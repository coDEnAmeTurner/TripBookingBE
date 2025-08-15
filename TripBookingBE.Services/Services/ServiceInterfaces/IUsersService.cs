using TripBookingBE.DTO.UserDTO;
using TripBookingBE.Models;

namespace TripBookingBE.Services.ServiceInterfaces;

public interface IUsersService
{
    Task<UserGetUsersDTO> GetUsers(string name = null, string type = null, string sellerCode = null, string email = null, string username = null);
    Task<UserGetByIdDTO> GetUserById(long id);
    Task<UserGetCreateOrUpdateModelDTO> GetCreateOrUpdateModel(long? id);
    Task<UserCreateOrUpdateDTO> CreateOrUpdate(User user, bool api = false);

    Task<UserDeleteDTO> DeleteUser(long id);

    Task<UserLoginDTO> LogUserIn(string username, string password);
    Task<UserLoginMVCDTO> LogUserInMVC(string username, string password);

    Task<UserHashDTO> Hash(string username, string password);
}