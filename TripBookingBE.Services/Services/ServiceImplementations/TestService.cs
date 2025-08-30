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

    public async Task<TestSendNewTaskDTO> SendNewTask(string args, string severity)
    {
        var dto = new TestSendNewTaskDTO();
        try
        {
            Console.WriteLine("RPC Client");
            string n = args.Length > 0 ? args : "30";
            var resp = await InvokeAsync(n);
            dto.Message = resp;
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

    private static async Task<string> InvokeAsync(string n)
    {
        var rpcClient = new RpcClient();
        await rpcClient.StartAsync();

        Console.WriteLine(" [x] Requesting fib({0})", n);
        var response = await rpcClient.CallAsync(n);
        return response;
    }
}