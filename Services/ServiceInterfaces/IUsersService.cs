using TripBookingBE.DTO.UserDTO;
using TripBookingBE.Models;

namespace TripBookingBE.Services.ServiceInterfaces;

public interface IUsersService
{
    Task<UserGetUsersDTO> GetUsers(string name = null, string type= null, string sellerCode= null, string email= null);
    Task<UserGetByIdDTO> GetUserById(long id);
    Task<UserGetCreateOrUpdateModelDTO> GetCreateOrUpdateModel(long? id);
    Task<UserCreateOrUpdateDTO> CreateOrUpdate(User user);

    Task<UserDeleteDTO> DeleteUser(long id);
}