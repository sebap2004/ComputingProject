using System.Security.Claims;
using System.Text.RegularExpressions;
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
    private List<TeacherQuestion> ActiveQuestions { get; set; } = new();
    public List<TeacherAnnouncement> Announcements { get; set; } = new();
    public string CurrentTask { get; set; }
    private string MessageInput;
    private MudTheme Theme = new MudTheme();
    private string UserName { get; set; }
    private string UserRole { get; set; }
    private List<string> QuestionIDs = new();
    private Dictionary<string, string> Answers = new();
    private string NotepadInput { get; set; }
    private string HandUpIcon => HasHandUp ? Icons.Material.Filled.Cancel : Icons.Material.Filled.FrontHand;
    private Color HandUpColor => HasHandUp ? Color.Error : Color.Primary;
    private bool HelpRequestAcknowledged { get; set; }
    public bool HasHandUp { get; set; }
    private bool IsConnected { get; set; }

    private string ConvertLinks(string text)
    {
        if (string.IsNullOrEmpty(text))
            return text;
        string pattern = @"(https?://[^\s]+)";
        var regex = new Regex(pattern);
        return regex.Replace(text, match =>
            $"<a style=\"text-decoration: underline;\" href=\"{match.Value}\" target=\"_blank\">{match.Value}</a>");
    }

    private async Task AskForHelp()
    {
        if (HasHandUp)
        {
            await ClassroomServer.CancelHelpRequest(UserName);
            HasHandUp = false;
            HelpRequestAcknowledged = false;
        }
        else
        {
            await ClassroomServer.SendHelpRequest(UserName);
            HasHandUp = true;
        }
    }

    protected override async Task OnInitializedAsync()
    {
        
        IsConnected = false;
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;
        if (user.Identity!.IsAuthenticated)
        {
            UserName = user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Unknown";
            UserRole = user.FindFirst(ClaimTypes.Role)?.Value ?? "No role";
            if (UserRole != "Student")
            {
                NavigationManager.NavigateTo($"/access-denied/{Uri.EscapeDataString($"incorrectrole")}");
            }

            await ConnectToHub();
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
        ClassroomService.OnResolveHelpRequest += ClassroomServiceOnOnResolveHelpRequest;
        ClassroomService.OnAcknowledgeHelpRequest += ClassroomServiceOnOnAcknowledgeHelpRequest;
        ClassroomService.OnReceiveAnnouncements += ClassroomServiceOnOnReceiveAnnouncements;
        ClassroomService.OnReceiveCurrentTask += ClassroomServiceOnOnReceiveCurrentTask;

        try
        {
            await ClassroomService.StartAsync();

            // Wait for the connection to be fully established
            while (!ClassroomService.IsConnected())
            {
                await Task.Delay(100); // Short delay to prevent tight loop
            }

            Console.WriteLine(ClassroomServer);
            Console.WriteLine(ClassroomService);

            Snackbar.Clear();
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopLeft;
            Snackbar.Add("Successfully connected to classroom as " + UserName, Severity.Success);
            await ClassroomServer.GetActiveQuestions();
            await ClassroomServer.GetClassroomState(UserName);
            await ClassroomServer.GetCurrentTask();
            await ClassroomServer.GetAnnouncements();
            IsConnected = true;
            StateHasChanged();
        }
        catch (Exception ex)
        {
            // Handle connection errors
            Snackbar.Add("Failed to connect to classroom: " + ex.Message, Severity.Error);
        }
    }

    private void ClassroomServiceOnOnReceiveCurrentTask(string obj)
    {
        CurrentTask = obj;
        StateHasChanged();
    }

    private void ClassroomServiceOnOnReceiveAnnouncements(List<TeacherAnnouncement> obj)
    {
        Announcements = obj;
        StateHasChanged();
    }

    private void ClassroomServiceOnOnAcknowledgeHelpRequest()
    {
        Console.WriteLine("Acknowledge Help Request");
        HelpRequestAcknowledged = true;
        StateHasChanged();
    }

    private void ClassroomServiceOnOnResolveHelpRequest()
    {
        Console.WriteLine("Resolve Help Request");
        Snackbar.Add("Your question has been answered!", Severity.Success);
        HelpRequestAcknowledged = false;
        HasHandUp = false;
        StateHasChanged();
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