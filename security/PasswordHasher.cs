using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;

namespace TripBookingBE.security;

public class PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 128 / 8;
    private const int KeySize = 256 / 8;
    private const int Iterations = 10000;

    private static readonly HashAlgorithmName hashAlgorithmName = HashAlgorithmName.SHA256;

    private const char Delimiter = ';';

    public string Hash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, hashAlgorithmName, KeySize);

        return string.Join(Delimiter, Convert.ToBase64String(salt), Convert.ToBase64String(hash));

    }

    public bool Verify(string hash, string password)
    {
        var elements = hash.Split(Delimiter);
        var salt = Convert.FromBase64String(elements[0]);
        var hashpart = Convert.FromBase64String(elements[1]);

        var hashinput = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, hashAlgorithmName, KeySize);

        return CryptographicOperations.FixedTimeEquals(hashpart, hashinput);
    }
}