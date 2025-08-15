using System.Net;
using TripBookingBE.Models;

namespace TripBookingBE.DTO.UserDTO;


public class UserLoginDTO
{
    public string AccessToken { get; set;} = null;
    public int RespCode { get; set; } = 200;
    public string Message { get; set; } = string.Empty;
}