using System.Security.Claims;

namespace ComputingProject.Client.Services;

// Responding to Server Messages
public interface IClassroomClient
{
    Task ReceiveMessage(string sender, string content, bool systemMessage);
    Task GetClassroomState(ClassroomState classroomState);
    Task GetStudents(List<string> students);
    Task SendStudentJoinedMessage(List<String> Users);
    Task SendStudentLeftMessage(List<String> Users);
}


// Send Message to Server
public interface IClassroomServer {
    Task SendMessage(string sender, string content, bool systemMessage);
    Task SendMessageToTeacher(string sender, string content, bool systemMessage);
    Task GetClassroomState(string classroomState);
    Task GetStudents();
    Task SetClassroomState(ClassroomState stateToChangeTo);
}

// Client Side Events
public interface IClassroomService
{
    event Action<string, string, bool> OnMessageReceived;
    event Action<ClassroomState> OnJoinGetState;
    event Action<ClassroomState> GetState;
    event Action<List<String>> OnStudentJoinedMessage;
    event Action<List<String>> OnStudentLeftMessage;
    event Action<List<String>> OnReceiveStudentList;
    Task StartAsync();
    Task StopAsync();
    bool IsConnected();
}