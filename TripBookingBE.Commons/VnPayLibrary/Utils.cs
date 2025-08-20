using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace TripBookingBE.Commons.VnPayLibrary;

public class Utils
{
    private readonly IHttpContextAccessor httpContextAccessor;

    public Utils(IHttpContextAccessor httpContextAccessor)
    {
        this.httpContextAccessor = httpContextAccessor;
    }

    public String HmacSHA512(string key, String inputData)
    {
        var hash = new StringBuilder();
        byte[] keyBytes = Encoding.UTF8.GetBytes(key);
        byte[] inputBytes = Encoding.UTF8.GetBytes(inputData);
        using (var hmac = new HMACSHA512(keyBytes))
        {
            byte[] hashValue = hmac.ComputeHash(inputBytes);
            foreach (var theByte in hashValue)
            {
                hash.Append(theByte.ToString("x2"));
            }
        }

        return hash.ToString();
    }
    public string GetIpAddress()
    {
        string ipAddress;
        try
        {
            ipAddress = httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
        }
        catch (Exception ex)
        {
            ipAddress = "Invalid IP:" + ex.Message;
        }

        return ipAddress;
    }
}