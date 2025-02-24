using System.Security.Claims;
using ComputingProject.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MudBlazor;

namespace ComputingProject.Client.Pages.Views;

public partial class StudentView : ComponentBase
{
    ClassroomState currentState;
    private List<Message> messages = new List<Message>();
    private string UserName { get; set; }
    private string UserRole { get; set; }
    private string MessageInput;
    
    
    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;
        if (user.Identity.IsAuthenticated)
        {
            UserName = user.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown";
            UserRole = user.FindFirst(ClaimTypes.Role)?.Value ?? "No role";
            if (UserRole != "Student")
            {
                NavigationManager.NavigateTo($"/access-denied/{Uri.EscapeDataString($"incorrectrole")}");
            }
            _ = ConnectToHub();
        }
        else
        {
            NavigationManager.NavigateTo($"/access-denied/{Uri.EscapeDataString("notauthenticated")}");
        }
    }

    public async Task ConnectToHub()
    {
        ClassroomService.OnMessageReceived += (puser, pmessage, system) =>
        {
            Message message = new Message()
            {
                user = puser,
                message = pmessage,
                systemMessage = system
            };
            messages.Add(message);
            
            StateHasChanged();
        };
        
        ClassroomService.OnJoinGetState += state =>
        {
            currentState = state;
            Snackbar.Add("HOly shit received the state: " + state, currentState == ClassroomState.Lecture ? Severity.Success : Severity.Warning);
        };
        
        try 
        {
            await ClassroomService.StartAsync();
    
            // Wait for the connection to be fully established
            while (!ClassroomService.IsConnected())
            {
                await Task.Delay(100);  // Short delay to prevent tight loop
            }

            Console.WriteLine(ClassroomServer);
            Console.WriteLine(ClassroomService);
    
            Snackbar.Clear();
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopLeft;
            Snackbar.Add("Successfully connected to classroom as " + UserName, Severity.Success);

            await InvokeAsync(StateHasChanged);
    
            await ClassroomServer.SendMessage(UserName, UserName + " has joined the classroom!", true);
        }
        catch (Exception ex)
        {
            // Handle connection errors
            Snackbar.Add("Failed to connect to classroom: " + ex.Message, Severity.Error);
        }
    }

    async Task Send()
    {
        if (string.IsNullOrWhiteSpace(MessageInput)) return;
    
        var tempMessage = MessageInput;
        MessageInput = string.Empty; // Clear input before sending
        StateHasChanged(); // Ensure UI updates before sending the message

        await ClassroomServer.SendMessage(UserName, tempMessage, false);
    }
    
    async Task SendToTeacher()
    {
        if (string.IsNullOrWhiteSpace(MessageInput)) return;
    
        var tempMessage = MessageInput;
        MessageInput = string.Empty; // Clear input before sending
        StateHasChanged(); // Ensure UI updates before sending the message

        await ClassroomServer.SendMessageToTeacher(UserName, tempMessage, false);
    }

    private async Task Enter(KeyboardEventArgs e)
    {
        if (e.Code == "Enter" || e.Code == "NumpadEnter")
        {
            await Send();
        }
    }

    public struct Message
    {
        public string user;
        public string message;
        public bool systemMessage;
    }
}