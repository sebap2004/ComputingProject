namespace ComputingProject.Client.Services;

// shared contracts for both client and server
public interface IClassroomClient
{
    Task ReceiveMessage(string sender, string content, bool systemMessage);
    Task GetClassroomState(ClassroomState classroomState);
}

public interface IClassroomServer {
    Task SendMessage(string sender, string content, bool systemMessage);
    Task GetClassroomState();
    Task SetClassroomState(ClassroomState stateToChangeTo);
}

public interface IClassroomService
{
    event Action<string, string, bool> OnMessageReceived;
    event Action<ClassroomState> OnJoinGetState;
    Task StartAsync();
    Task StopAsync();
    bool IsConnected();
}