using TripBookingBE.Commons.DTO.TestDTO;
using TripBookingBE.Services.ServiceInterfaces;
using RabbitMQ.Client;
using System.Text;

namespace TripBookingBE.Services.ServiceImplementations;


public class TestService : ITestService
{
    public async Task<TestSendHWDTO> SendHelloWorld()
    {
        var dto = new TestSendHWDTO();

        var factory = new ConnectionFactory { HostName = "localhost", Port = 6080 };
        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();      

        await channel.QueueDeclareAsync(queue: "hello", durable: false, exclusive: false, autoDelete: false,
    arguments: null);

        const string message = "Hello World!";
        var body = Encoding.UTF8.GetBytes(message);
        await channel.BasicPublishAsync(exchange: string.Empty, routingKey: "hello", body: body);

        return dto;
    }
}