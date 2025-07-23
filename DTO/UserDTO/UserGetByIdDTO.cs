using System.Net;
using TripBookingBE.Models;

namespace TripBookingBE.DTO.UserDTO;

public class UserGetByIdDTO
{
    public User User { get; set; } = null;
    public HttpStatusCode StatusCode = HttpStatusCode.OK;
    public string Message = string.Empty;
}