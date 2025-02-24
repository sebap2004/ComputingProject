using System.Security.Claims;
using ComputingProject.Client.Services;
using ComputingProject.Services;
using Microsoft.AspNetCore.SignalR;

namespace ComputingProject.Hubs;


public class Classroom : Hub<IClassroomClient>, IClassroomServer
{
    private readonly ClassroomStateService ClassroomStateService;
    public Classroom(ClassroomStateService stateService)
    {
        ClassroomStateService = stateService;
    }
    
    public override Task OnConnectedAsync()
    {
        Console.WriteLine("New Connection");
        if (Context.User != null)
        {
            string? Role = Context.User.FindFirst(ClaimTypes.Role)?.Value;
            Console.WriteLine("Role: " + Role);
            if (Role == "Student")
            {
                ClassroomStateService.AddStudent(Context.User.Identity.Name);
                Clients.Group("Teacher").SendStudentJoinedMessage(ClassroomStateService.ConnectedUsers);
                Console.WriteLine("sent student joined teacher");
            }
            if (Role  == "Teacher")
            {
                Groups.AddToGroupAsync(Context.ConnectionId, "Teacher");
                Console.WriteLine("added teacher to group");
            }
        }
        return Task.CompletedTask;
    }
    
    

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        Console.WriteLine("New Disconnection");
        if (Context.User != null)
        {
            ClassroomStateService.RemoveStudent(Context.User.Identity.Name);
            if (Context.User.FindFirst(ClaimTypes.Role)?.Value == "Student")
            {
                Clients.Group("Teacher").SendStudentLeftMessage(ClassroomStateService.ConnectedUsers);
            }
        }
        return Task.CompletedTask;
    }

    public async Task SendMessage(string sender, string content, bool systemMessage)
    {
        await Clients.All.ReceiveMessage(sender, content, systemMessage);
    }
    
    public async Task SendMessageToTeacher(string sender, string content, bool systemMessage)
    {
        await Clients.User("Teacher").ReceiveMessage(sender + " - TEACHER ONLY", content, systemMessage);
        await Clients.User(sender).ReceiveMessage(sender, content, systemMessage);
    }

    public async Task GetClassroomState(string UserName)
    {
        Console.WriteLine("Sent state to clients");
        await Clients.User(UserName).GetClassroomState(ClassroomStateService.classroomState);
    }

    public async Task GetStudents()
    {
        Console.WriteLine("Get student list, count: " + ClassroomStateService.ConnectedUsers.Count);
        await Clients.Group("Teacher").GetStudents(ClassroomStateService.ConnectedUsers);
    }

    public async Task SetClassroomState(ClassroomState stateToChangeTo)
    {
        ClassroomStateService.classroomState = stateToChangeTo;
        await Clients.All.GetClassroomState(ClassroomStateService.classroomState);
        await Clients.All.ReceiveMessage("", "Set the classroom state to " + stateToChangeTo, true);
    }
}