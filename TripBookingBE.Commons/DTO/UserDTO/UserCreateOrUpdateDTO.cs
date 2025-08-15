using System.Net;
using TripBookingBE.Models;

namespace TripBookingBE.DTO.UserDTO;

public class UserCreateOrUpdateDTO
{
    public User User { get; set; } = null;
    public HttpStatusCode RespCode { get; set; } = HttpStatusCode.Created;
    public string Message { get; set; } = string.Empty;
}