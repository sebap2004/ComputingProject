using System.Security.Claims;
using ComputingProject.Client.Dialogs;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;

namespace ComputingProject.Client.Pages;

public partial class Home
{
    private string? UserName {get; set;}
    private string? UserRole {get; set;}
    private bool IsAuthenticated {get; set;}
    private bool HasReadDocuments {get; set;}
    private bool HasReadPIS {get; set;}
    private bool HasReadConsentForm {get; set;}
    private bool HasAgreedToConditions {get; set;}
    
    protected override async Task OnInitializedAsync()
    {
        AuthStateProvider.AuthenticationStateChanged += AuthStateProviderOnAuthenticationStateChanged;
        var authState = await AuthStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;
        if (user.Identity is { IsAuthenticated: true })
        {
            UserName = user.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown";
            UserRole = user.FindFirst(ClaimTypes.Role)?.Value ?? "No role";
            IsAuthenticated = true;
        }
    }

    private async void AuthStateProviderOnAuthenticationStateChanged(Task<AuthenticationState> task)
    {
        try
        {
            var authState = await task;
            if (authState.User.Identity is not null)
            {
                IsAuthenticated = authState.User.Identity.IsAuthenticated;
                UserName = authState.User.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown";
                UserRole = authState.User.FindFirst(ClaimTypes.Role)?.Value ?? "No role";
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error fetching authentication state: {e.Message}");
        }
    }

    private Task OpenStudentDialog()
    {
        if (IsAuthenticated && UserRole == "Student")
        {
            NavigationManager.NavigateTo("/student");
            return Task.CompletedTask;
        }
        var options = new DialogOptions { CloseOnEscapeKey = true };
        return DialogService.ShowAsync<StudentLoginDialog>("Student Login", options);
    }
    
    private Task OpenTeacherDialog()
    {
        if (IsAuthenticated && UserRole == "Teacher")
        {
            NavigationManager.NavigateTo("/teacher");
            return Task.CompletedTask;
        }
        var options = new DialogOptions { CloseOnEscapeKey = true };
        return DialogService.ShowAsync<TeacherLoginDialog>("Teacher Login", options);
    }
}