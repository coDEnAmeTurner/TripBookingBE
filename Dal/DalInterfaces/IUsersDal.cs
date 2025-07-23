using TripBookingBE.DTO.UserDTO;
using TripBookingBE.Models;

namespace TripBookingBE.Dal.DalInterfaces;

public interface IUsersDal
{
    Task<UserGetUsersDTO> GetUsers(string name, string type, string sellerCode, string email);
    Task<UserGetByIdDTO> GetUserById(long id);
    Task<UserDeleteDTO> DeleteUser(long id);
    Task<UserCreateOrUpdateDTO> Update(User user);
    Task<UserCreateOrUpdateDTO> Create(User user);
}