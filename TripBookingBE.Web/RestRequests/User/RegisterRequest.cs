namespace TripBookingBE.RestRequests;

public class RegisterRequest
{
    public IFormFile? File { get; set; }
    public string? Password { get; set; }
    public string? UserName { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public bool? Active { get; set; }
    public string? Name { get; set; }
    public string? Phone { get; set; }
    public string? Type { get; set; }
    public string? SellerCode { get; set; }

}