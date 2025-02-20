namespace ComputingProject.Client.Services;

// shared contracts for both client and server
public interface IClassroomClient
{
    Task ReceiveMessage(string sender, string content, bool systemMessage);
}

public interface IClassroomServer {
    Task SendMessage(string sender, string content, bool systemMessage);
}

public interface IClassroomService
{
    event Action<string, string, bool> OnMessageReceived;
    Task StartAsync();
    Task StopAsync();
    bool IsConnected();
}