public class MyDependency : IMyDependency
{
    public void WriteMessage(string message)
    {
        // Implementation of the method to write a message
        Console.WriteLine($"MyDependency.WriteMessage Message: {message}");
    }
}