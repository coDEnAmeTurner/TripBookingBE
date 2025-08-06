namespace TripBookingBE.RestRequests;

public class RegisterRequest
{
    public IFormFile? File { get; set; }
    public string? Password { get; set; }
    public string UserName { get; set; } = null!;
    public string? FirstName { get; set; }
    public string LastName { get; set; } = null!;
    public string? Email { get; set; }
    public bool Active { get; set; }
    public string Name { get; set; } = null!;
    public string? Phone { get; set; }
    public string Type { get; set; } = null!;
    public string? SellerCode { get; set; }

}