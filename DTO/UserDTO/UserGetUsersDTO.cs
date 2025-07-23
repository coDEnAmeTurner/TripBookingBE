using System.Net;
using TripBookingBE.Models;

namespace TripBookingBE.DTO.UserDTO;


public class UserGetUsersDTO
{
    public List<User> Users { get; set; } = null;
    public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;
    public string Message { get; set; } = string.Empty;
}