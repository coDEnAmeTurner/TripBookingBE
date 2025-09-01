namespace TripBookingBE.Commons.Configurations;

public class RabbitMqConfigs
{
    public string HostName { get; set; } = "";
    public int Port { get; set; }
    public string EmailQueueName { get; set; } = "";
}
