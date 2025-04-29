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
    /// <summary>
    /// Current cached classroom state. Updated from server
    /// </summary>
    ClassroomState CurrentClassroomState;
    
    /// <summary>
    /// Cached list of active questions. Updated from server
    /// </summary>
    private List<TeacherQuestion> ActiveQuestions { get; set; } = new();
    
    /// <summary>
    /// Cached list of announcements. Updated from server
    /// </summary>
    public List<TeacherAnnouncement> Announcements { get; set; } = new();
    
    /// <summary>
    /// Current cached task. Updated from server
    /// </summary>
    public string CurrentTask { get; set; }
    
    /// <summary>
    /// Theme of the page
    /// </summary>
    private MudTheme Theme = new MudTheme();
    
    
    /// <summary>
    /// Cached username retrieved from authentication state provider.
    /// </summary>
    private string UserName { get; set; }
    
    /// <summary>
    /// Cached user role retrieved from authentication state provider.
    /// </summary>
    private string UserRole { get; set; }
    
    /// <summary>
    /// Dictionary of answers to active questions. Stores the question ID as key and the answer as value.
    /// Used to store the answer to a question when the user clicks the "send" button.
    /// </summary>
    private Dictionary<string, string> Answers = new();

    /// <summary>
    /// Icon displayed on the "Ask For Help" button, changing based on the student's hand up status.
    /// </summary>
    private string HandUpIcon => HasHandUp ? Icons.Material.Filled.Cancel : Icons.Material.Filled.FrontHand;
    
    /// <summary>
    /// Color of the "Ask For Help" button, changing based on the student's hand up status.'
    /// </summary>
    private Color HandUpColor => HasHandUp ? Color.Error : Color.Primary;
    
    /// <summary>
    /// Cached status of whether a help request has been acknowledged by the teacher.
    /// </summary>
    private bool HelpRequestAcknowledged { get; set; }
    
    /// <summary>
    /// If the student has hand up to the teacher.
    /// </summary>
    public bool HasHandUp { get; set; }
    
    /// <summary>
    /// If the student is connected to the classroom.
    /// </summary>
    private bool IsConnected { get; set; }

    /// <summary>
    /// Converts links into a markup string to be rendered in the view
    /// </summary>
    /// <param name="text">Text to convert</param>
    /// <returns>Marked up string</returns>
    private string ConvertLinks(string text)
    {
        if (string.IsNullOrEmpty(text))
            return text;
        string pattern = @"(https?://[^\s]+)";
        var regex = new Regex(pattern);
        return regex.Replace(text, match =>
            $"<a style=\"text-decoration: underline;\" href=\"{match.Value}\" target=\"_blank\">{match.Value}</a>");
    }

    /// <summary>
    /// Asks the teacher for help. Makes a call to the server to send a help request.
    /// </summary>
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

    /// <summary>
    /// Connects to the classroom hub. Called when the page is initialized.
    /// </summary>
    private async Task ConnectToHub()
    {
        ClassroomService.GetState += OnGetClassroomState;
        ClassroomService.OnReceiveActiveQuestions += ReceiveActiveQuestions;
        ClassroomService.OnResolveHelpRequest += OnResolveHelpRequest;
        ClassroomService.OnAcknowledgeHelpRequest += OnHelpRequestAcknowledged;
        ClassroomService.OnReceiveAnnouncements += ReceiveAnnouncements;
        ClassroomService.OnReceiveCurrentTask += ReceiveCurrentTask;

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
            await ClassroomServer.GetClassroomState();
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

    /// <summary>
    /// Event function that is called when the classroom service receives a message from the server.
    /// </summary>
    /// <param name="obj"></param>
    private void ReceiveCurrentTask(string obj)
    {
        CurrentTask = obj;
        StateHasChanged();
    }

    /// <summary>
    /// Event function that is called when the classroom service receives an announcement from the server.
    /// </summary>
    /// <param name="obj">List of announcements from the server</param>
    private void ReceiveAnnouncements(List<TeacherAnnouncement> obj)
    {
        Announcements = obj;
        StateHasChanged();
    }

    /// <summary>
    /// Event function that is called when the classroom service acknowledges this client's help request.
    /// </summary>
    private void OnHelpRequestAcknowledged()
    {
        Console.WriteLine("Acknowledge Help Request");
        HelpRequestAcknowledged = true;
        StateHasChanged();
    }

    /// <summary>
    /// Event function that is called when the classroom service resolves this client's help request.
    /// </summary>
    private void OnResolveHelpRequest()
    {
        Console.WriteLine("Resolve Help Request");
        Snackbar.Add("Your question has been answered!", Severity.Success);
        HelpRequestAcknowledged = false;
        HasHandUp = false;
        StateHasChanged();
    }

    /// <summary>
    /// Event function that is called when the classroom service receives a list of active questions from the server.
    /// </summary>
    /// <param name="obj">List of active questions</param>
    private void ReceiveActiveQuestions(List<TeacherQuestion> obj)
    {
        ActiveQuestions = obj;
        Answers.Clear();
        foreach (var question in ActiveQuestions)
        {
            Answers.Add(question.Id, "");
        }

        StateHasChanged();
        Console.WriteLine("NEW DICTIONARY VALUES:");
        foreach (var epic in Answers)
        {
            Console.WriteLine(epic.Key + ": " + epic.Value);
        }
    }

    /// <summary>
    /// Event function that gets the classroom's state
    /// </summary>
    /// <param name="state">State recieved</param>
    private void OnGetClassroomState(ClassroomState state)
    {
        CurrentClassroomState = state;
        StateHasChanged();
    }

    /// <summary>
    /// Responds to a question and sends response to the server.
    /// </summary>
    /// <param name="questionID">Question to add a response to</param>
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

    /// <summary>
    /// Keyboard event handler called whenever a key is pressed.
    /// If the enter key is pressed, the user sends a response to the question.
    /// </summary>
    /// <param name="e">Event object</param>
    /// <param name="questionID">question ID to respond to</param>
    private async Task Enter(KeyboardEventArgs e, string questionID)
    {
        if (e.Code == "Enter" || e.Code == "NumpadEnter")
        {
            await RespondToQuestion(questionID);
        }
    }
}