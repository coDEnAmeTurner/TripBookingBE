using TripBookingBE.DTO;
using TripBookingBE.Models;

namespace TripBookingBE.Dal.DalInterfaces;

public interface IUsersDal
{
    UserGetUsersDTO GetUsers(string name, string type, string sellerCode, string email);
    Task<UserGetByIdDTO> GetUserById(long id);
    Task<UserDeleteDTO> DeleteUser(long id);
    Task<UserCreateOrUpdateDTO> Update(long id, string Password, string UserName, string FirstName, string LastName, string Email, bool Active, string Name, string Phone, string Type, string SellerCode, string Avatar);
    Task<UserCreateOrUpdateDTO> Create(string Password, string UserName, string FirstName, string LastName, string Email, bool Active, string Name, string Phone, string Type, string SellerCode, string Avatar);
}