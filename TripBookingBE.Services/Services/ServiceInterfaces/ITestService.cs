using TripBookingBE.Commons.DTO.TestDTO;

namespace TripBookingBE.Services.ServiceInterfaces;

public interface ITestService
{
    Task<TestSendHWDTO> SendHelloWorld();
    Task<TestSendNewTaskDTO> SendNewTask(string args, string severity);
}