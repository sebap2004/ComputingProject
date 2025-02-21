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
    
    public async Task SendMessage(string sender, string content, bool systemMessage)
    {
        await Clients.All.ReceiveMessage(sender, content, systemMessage);
    }

    public async Task GetClassroomState()
    {
        Console.WriteLine("Sent state to clients");
        await Clients.All.GetClassroomState(ClassroomStateService.classroomState);
    }

    public async Task SetClassroomState(ClassroomState stateToChangeTo)
    {
        ClassroomStateService.classroomState = stateToChangeTo;
        await Clients.All.GetClassroomState(ClassroomStateService.classroomState);
    }
}