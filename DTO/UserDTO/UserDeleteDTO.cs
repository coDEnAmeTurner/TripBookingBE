using System.Net;
using TripBookingBE.Models;

namespace TripBookingBE.DTO.UserDTO;

public class UserDeleteDTO
{
    public User User { get; set; } = null;
    public HttpStatusCode RespCode = HttpStatusCode.NoContent;
    public string Message = string.Empty;
}