namespace TripBookingBE.security;

public interface IPasswordHasher
{
    public string Hash(string password);    

    public bool Verify(string hash, string password);
}