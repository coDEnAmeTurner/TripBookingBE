using System.Text.Json;
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

    private readonly RpcClient rpcClient;

    public EmailService(IOptions<SendGridConfigs> sendGridConfigs, ISendGridClient sendGridClient, RpcClient rpcClient)
    {
        this.sendGridConfigs = sendGridConfigs.Value;
        this.sendGridClient = sendGridClient;
        this.rpcClient = rpcClient;
    }

    public async Task<EmailSendDTO> SendMail(string toEmail, string subject, string htmlbody, string plainText)
    {
        // try
        // {
        //     var msg = new SendGridMessage()
        //     {
        //         From = new EmailAddress(sendGridConfigs.FromEmail, sendGridConfigs.FromName),
        //         Subject = subject,
        //         HtmlContent = htmlbody,
        //         PlainTextContent = plainText
        //     };
        //     msg.AddTo(new EmailAddress(toEmail));
        //     var response = await sendGridClient.SendEmailAsync(msg);
        //     if (!response.IsSuccessStatusCode)
        //     {
        //         dto.RespCode = (int)response.StatusCode;
        //         dto.Message = response.Body.ToString();
        //     }
        // }
        // catch (Exception ex)
        // {
        //     dto.RespCode = 500;
        //     dto.Message = $"{ex.Message}\t{ex.InnerException.Message}";
        // }

        var brokerreq = new EmailSendBrokerDTO()
        {
            ToEmail = toEmail,
            Subject = subject,
            Htmlbody = htmlbody,
            PlainText = plainText
        };
        string message = JsonSerializer.Serialize(brokerreq);
        await rpcClient.StartAsync();

        var resp = await rpcClient.CallAsync(message);
        var respobj = JsonSerializer.Deserialize<EmailSendDTO>(resp);

        return respobj;
    }
}