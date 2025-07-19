using TripBookingBE.DTO;
using TripBookingBE.Models;

namespace TripBookingBE.Services.ServiceInterfaces;

public interface IUsersService
{
    IQueryable<User> GetUsers(string name, string type, string sellerCode, string email);
    Task<User> GetUserById(long id);
    Task<User> GetCreateOrUpdateModel(long? id);
    Task<UserCreateOrUpdateDTO> CreateOrUpdate(long? id, string Password, string NewPassword, string ConfirmPassword, string UserName, string FirstName, string LastName, string Email, bool Active, string Name, string Phone, string Type, string SellerCode, IFormFile file);

}