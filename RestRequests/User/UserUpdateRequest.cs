namespace TripBookingBE.RestRequests;

public class UserUpdateRequest : RegisterRequest
{
    public string? PasswordHash { get; set; }
    public string? NewPassword { get; set; }
    public string? ConfirmPassword { get; set; }
}