public class MyDependency2 : IMyDependency
{
    private readonly ILogger<MyDependency2> _logger;

    public MyDependency2(ILogger<MyDependency2> logger)
    {
        _logger = logger;
    }

    public void WriteMessage(string message)
    {
        // Implementation of the method to write a message
        Console.WriteLine($"MyDependency2.WriteMessage Message: {message}");
    }
}