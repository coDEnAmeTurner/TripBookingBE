using System.Net;
using TripBookingBE.Models;

namespace TripBookingBE.DTO.UserDTO;


public class UserLoginDTO
{
    public string AccessToken { get; set;} = null;
    public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;
    public string Message { get; set; } = string.Empty;
}