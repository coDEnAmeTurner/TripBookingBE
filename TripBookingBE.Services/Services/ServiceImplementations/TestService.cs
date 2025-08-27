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
        try
        {
            var factory = new ConnectionFactory { HostName = "localhost", Port = 6078 };
            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(queue: "hello", durable: false, exclusive: false, autoDelete: false,
        arguments: null);

            const string message = "Hello World!";
            var body = Encoding.UTF8.GetBytes(message);
            await channel.BasicPublishAsync(exchange: string.Empty, routingKey: "hello", body: body);
        }
        catch (Exception ex)
        {
            dto.RespCode = 500;
            dto.Message = $"{ex.Message};{ex.InnerException.Message}";
        }

        return dto;
    }

    public async Task<TestSendNewTaskDTO> SendNewTask(string args)
    {
        var dto = new TestSendNewTaskDTO();
        try
        {
            var factory = new ConnectionFactory { HostName = "localhost", Port = 6078 };
            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

    //         await channel.QueueDeclareAsync(queue: "task_queue", durable: true, exclusive: false,
    // autoDelete: false, arguments: null);
            await channel.ExchangeDeclareAsync(exchange: "logs", type: ExchangeType.Fanout);

            string message = GetMessage(args);
            var body = Encoding.UTF8.GetBytes(message);
            var properties = new BasicProperties
            {
                Persistent = true
            };
            await channel.BasicPublishAsync(exchange: "logs", routingKey: "", mandatory: true, basicProperties: properties, body: body);
        }
        catch (Exception ex)
        {
            dto.RespCode = 500;
            dto.Message = $"{ex.Message};{ex.InnerException.Message}";
        }

        return dto;
    }

    private string GetMessage(string args)
    {
        return (args.Length > 0) ? args : "Hello World!";
    }
}