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
    ClassroomState CurrentClassroomState;
    public List<TeacherQuestion> ActiveQuestions { get; set; } = new ();
    private string MessageInput;
    private MudTheme Theme = new MudTheme();
    private string UserName { get; set; }
    private string UserRole { get; set; }

    private List<string> QuestionIDs = new();
    private Dictionary<string, string> Answers = new();
    
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

    private async Task ConnectToHub()
    {
        ClassroomService.GetState += OnClassroomServiceOnGetState;
        ClassroomService.OnReceiveActiveQuestions += ReceiveActiveQuestions;
        
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
            await ClassroomServer.GetActiveQuestions();
        }
        catch (Exception ex)
        {
            // Handle connection errors
            Snackbar.Add("Failed to connect to classroom: " + ex.Message, Severity.Error);
        }
    }
    
    private void ReceiveActiveQuestions(List<TeacherQuestion> obj)
    {
        ActiveQuestions = obj;
        Answers.Clear();
        QuestionIDs.Clear();
        foreach (var question in ActiveQuestions)
        {
            Answers.Add(question.Id, "");
            QuestionIDs.Add(question.Id);
        }
        StateHasChanged();
        Console.WriteLine("NEW DICTIONARY VALUES:");
        foreach (var epic in Answers)
        {
            Console.WriteLine(epic.Key + ": " + epic.Value);
        }
    }
    
    private void OnClassroomServiceOnGetState(ClassroomState state)
    {
        CurrentClassroomState = state;
        StateHasChanged();
    }
    
    async Task RespondToQuestion(string questionID)
    {
        Console.WriteLine("Trying to respond to question with id: " + questionID);
        string answer = Answers[questionID];

        if (string.IsNullOrWhiteSpace(answer))
        {
            
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomCenter;
            Snackbar.Add("Box cannot be empty.", Severity.Error);
            return;
        }
        
        await ClassroomServer.AnswerTeacherQuestion(UserName, questionID, answer);
    }

    private async Task Enter(KeyboardEventArgs e, string questionID)
    {
        if (e.Code == "Enter" || e.Code == "NumpadEnter")
        {
            await RespondToQuestion(questionID);
        }
    }
}