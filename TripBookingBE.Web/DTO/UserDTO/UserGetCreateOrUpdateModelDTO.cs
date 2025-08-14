using System.Net;
using TripBookingBE.Models;

namespace TripBookingBE.DTO.UserDTO;


public class UserGetCreateOrUpdateModelDTO
{
    public User User { get; set; } = null;
    public HttpStatusCode RespCode { get; set; } = HttpStatusCode.OK;
    public string Message { get; set; } = string.Empty;
}