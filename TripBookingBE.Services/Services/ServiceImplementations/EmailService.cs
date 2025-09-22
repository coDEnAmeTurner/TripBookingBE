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

    private readonly IRpcClient rpcClient;

    public EmailService(IOptions<SendGridConfigs> sendGridConfigs, ISendGridClient sendGridClient, IRpcClient rpcClient)
    {
        this.sendGridConfigs = sendGridConfigs.Value;
        this.sendGridClient = sendGridClient;
        this.rpcClient = rpcClient;
    }

    public async Task<EmailSendDTO> SendMail(string toEmail, string subject, string htmlbody, string plainText)
    {
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