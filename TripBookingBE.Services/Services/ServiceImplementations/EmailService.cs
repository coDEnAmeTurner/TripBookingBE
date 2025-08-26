using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using TripBookingBE.Commons.Configurations;
using TripBookingBE.Commons.DTO.EmailDTO;
using TripBookingBE.Services.ServiceInterfaces;

namespace TripBookingBE.Services.ServiceImplementations;

public class EmailService : IEmailService
{
    private readonly SendGridConfigs sendGridConfigs;
    private readonly ISendGridClient sendGridClient;

    public EmailService(IOptions<SendGridConfigs> sendGridConfigs, ISendGridClient sendGridClient)
    {
        this.sendGridConfigs = sendGridConfigs.Value;
        this.sendGridClient = sendGridClient;
    }

    public async Task<EmailSendDTO> SendMail(string toEmail, string subject, string htmlbody, string plainText)
    {
        var dto = new EmailSendDTO();

        try
        {
            var msg = new SendGridMessage()
            {
                From = new EmailAddress(sendGridConfigs.FromEmail, sendGridConfigs.FromName),
                Subject = subject,
                HtmlContent = htmlbody,
                PlainTextContent = plainText
            };
            msg.AddTo(new EmailAddress(toEmail));
            var response = await sendGridClient.SendEmailAsync(msg);
            if (!response.IsSuccessStatusCode)
            {
                dto.RespCode = (int)response.StatusCode;
                dto.Message = response.Body.ToString();
            }
        }
        catch (Exception ex)
        {
            dto.RespCode = 500;
            dto.Message = $"{ex.Message}\t{ex.InnerException.Message}";
        }

        return dto;
    }
}