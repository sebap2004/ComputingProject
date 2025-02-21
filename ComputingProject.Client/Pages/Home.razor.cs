using System.Security.Claims;
using ComputingProject.Client.Dialogs;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;

namespace ComputingProject.Client.Pages;

public partial class Home
{
    private string UserName {get; set;}
    private string UserRole {get; set;}
    
    private bool IsAuthenticated {get; set;}
    
    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;
        if (user.Identity.IsAuthenticated)
        {
            UserName = user.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown";
            UserRole = user.FindFirst(ClaimTypes.Role)?.Value ?? "No role";
            IsAuthenticated = true;
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
        return DialogService.ShowAsync<StudentLoginDialog>("Simple Dialog", options);
    }
    
    private Task OpenTeacherDialog()
    {
        if (IsAuthenticated && UserRole == "Teacher")
        {
            NavigationManager.NavigateTo("/teacher");
            return Task.CompletedTask;
        }
        var options = new DialogOptions { CloseOnEscapeKey = true };
        return DialogService.ShowAsync<TeacherLoginDialog>("Simple Dialog", options);
    }
}