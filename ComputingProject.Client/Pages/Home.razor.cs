using ComputingProject.Client.Dialogs;
using MudBlazor;

namespace ComputingProject.Client.Pages;

public partial class Home
{
    private Task OpenStudentDialog()
    {
        var options = new DialogOptions { CloseOnEscapeKey = true };

        return DialogService.ShowAsync<StudentLoginDialog>("Simple Dialog", options);
    }
    
    private Task OpenTeacherDialog()
    {
        var options = new DialogOptions { CloseOnEscapeKey = true };

        return DialogService.ShowAsync<TeacherLoginDialog>("Simple Dialog", options);
    }
}