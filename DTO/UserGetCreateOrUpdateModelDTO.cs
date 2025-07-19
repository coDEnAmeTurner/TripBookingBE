using System.Net;
using TripBookingBE.Models;

namespace TripBookingBE.DTO;

public class UserGetCreateOrUpdateModelDTO
{
    public User User { get; set; } = null;
    public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;
    public string Message { get; set; } = string.Empty;
}