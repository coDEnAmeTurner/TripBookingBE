using TripBookingBE.Commons.DTO.EmailDTO;

namespace TripBookingBE.Services.ServiceInterfaces;

public interface IEmailService
{
    Task<EmailSendDTO> SendMail(string toEmail, string subject, string htmlbody, string plainText);
    
}