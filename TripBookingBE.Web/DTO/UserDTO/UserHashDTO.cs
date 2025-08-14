using TripBookingBE.Models;

namespace TripBookingBE.DTO.UserDTO;

public class UserHashDTO
{
    public string Hash { get; set; } = null;

    public User User {get;set;}=null;
    public int RespCode = 200;
    public string Message { get; set; } = null;
}