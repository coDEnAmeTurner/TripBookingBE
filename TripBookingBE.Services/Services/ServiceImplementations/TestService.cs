using TripBookingBE.Commons.DTO.TestDTO;
using TripBookingBE.Services.ServiceInterfaces;
using RabbitMQ.Client;
using System.Text;
using System.Buffers.Binary;

namespace TripBookingBE.Services.ServiceImplementations;


public class TestService : ITestService
{
    const ushort MAX_OUTSTANDING_CONFIRMS = 256;

    const int MESSAGE_COUNT = 50_000;

    const string queueName = "logs";

    CreateChannelOptions channelOpts;
    TaskCompletionSource<bool> allMessagesConfirmedTcs;
    LinkedList<ulong> outstandingConfirms;
    SemaphoreSlim semaphore;
    int confirmedCount = 0;
    List<ValueTuple<ulong, ValueTask>> publishTasks;
    public TestService()
    {
        channelOpts = new CreateChannelOptions(
                publisherConfirmationsEnabled: true,
                publisherConfirmationTrackingEnabled: true,
                outstandingPublisherConfirmationsRateLimiter: new ThrottlingRateLimiter(MAX_OUTSTANDING_CONFIRMS)
            );

        allMessagesConfirmedTcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        outstandingConfirms = new LinkedList<ulong>();
        semaphore = new SemaphoreSlim(1, 1);
        publishTasks = new List<ValueTuple<ulong, ValueTask>>();
    }

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
            await using IConnection connection = await CreateConnectionAsync();
            await using IChannel channel = await connection.CreateChannelAsync(channelOpts);

            var props = new BasicProperties
            {
                Persistent = true
            };

            await channel.QueueDeclareAsync(queue: queueName, durable: true, exclusive: false,
    autoDelete: false, arguments: null);

            AssignChannelCallbacks(channel);

            await SendMessage(args, channel, props);

            await LogSendTaskStatus();

            await CheckClearConfirmQueueUnderUnitOfTime();

        }
        catch (Exception ex)
        {
            dto.RespCode = 500;
            dto.Message = $"{ex.Message};{ex.InnerException?.Message}";
        }

        return dto;
    }

    private async Task CheckClearConfirmQueueUnderUnitOfTime()
    {
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));

        try
        {
            await allMessagesConfirmedTcs.Task.WaitAsync(cts.Token);
        }
        catch (OperationCanceledException)
        {
            Console.Error.WriteLine("{0} [ERROR] all messages could not be published and confirmed within 10 seconds", DateTime.Now);
        }
        catch (TimeoutException)
        {
            Console.Error.WriteLine("{0} [ERROR] all messages could not be published and confirmed within 10 seconds", DateTime.Now);
        }

    }

    private async Task LogSendTaskStatus()
    {
        foreach ((ulong SeqNo, ValueTask PublishTask) datum in publishTasks)
        {
            try
            {
                await datum.PublishTask;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"{DateTime.Now} [ERROR] saw nack, seqNo: '{datum.SeqNo}', ex: '{ex}'");
            }
        }
    }

    private async Task SendMessage(string args, IChannel channel, BasicProperties props)
    {
        string msg = args;
        byte[] body = Encoding.UTF8.GetBytes(msg);
        ulong nextPublishSeqNo = await channel.GetNextPublishSequenceNumberAsync();

        await semaphore.WaitAsync();

        try
        {
            outstandingConfirms.AddLast(nextPublishSeqNo);
        }
        finally
        {
            semaphore.Release();
        }

        string rk = queueName;
        (ulong, ValueTask) data =
            (nextPublishSeqNo, channel.BasicPublishAsync(exchange: string.Empty, routingKey: rk, body: body, mandatory: true, basicProperties: props));
        publishTasks.Add(data);
    }

    private void AssignChannelCallbacks(IChannel channel)
    {
        channel.BasicReturnAsync += (sender, ea) =>
        {
            ulong sequenceNumber = 0;

            IReadOnlyBasicProperties props = ea.BasicProperties;
            if (props.Headers is not null)
            {
                object? maybeSeqNum = props.Headers[Constants.PublishSequenceNumberHeader];
                if (maybeSeqNum is not null)
                {
                    sequenceNumber = BinaryPrimitives.ReadUInt64BigEndian((byte[])maybeSeqNum);
                }
            }

            Console.WriteLine($"{DateTime.Now} [WARNING] message sequence number {sequenceNumber} has been basic.return-ed");
            return CleanOutstandingConfirms(sequenceNumber, false);
        };

        channel.BasicAcksAsync += (sender, ea) => CleanOutstandingConfirms(ea.DeliveryTag, ea.Multiple);
        channel.BasicNacksAsync += (sender, ea) =>
        {
            Console.WriteLine($"{DateTime.Now} [WARNING] message sequence number: {ea.DeliveryTag} has been nacked (multiple: {ea.Multiple})");
            return CleanOutstandingConfirms(ea.DeliveryTag, ea.Multiple);
        };
    }

    async Task CleanOutstandingConfirms(ulong deliveryTag, bool multiple)
    {
        await semaphore.WaitAsync();
        try
        {
            if (multiple)
            {
                do
                {
                    LinkedListNode<ulong>? node = outstandingConfirms.First;
                    if (node is null)
                    {
                        break;
                    }
                    if (node.Value <= deliveryTag)
                    {
                        outstandingConfirms.RemoveFirst();
                    }
                    else
                    {
                        break;
                    }

                    confirmedCount++;
                } while (true);
            }
            else
            {
                confirmedCount++;
                outstandingConfirms.Remove(deliveryTag);
            }
        }
        finally
        {
            semaphore.Release();
        }

        if (outstandingConfirms.Count == 0 || confirmedCount == MESSAGE_COUNT)
        {
            allMessagesConfirmedTcs.SetResult(true);
        }
    }

    Task<IConnection> CreateConnectionAsync()
    {
        var factory = new ConnectionFactory { HostName = "localhost", Port = 6078 };
        return factory.CreateConnectionAsync();
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