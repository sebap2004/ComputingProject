using ComputingProject.Client.Dialogs;
using MudBlazor;

namespace ComputingProject.Client.Pages;

public partial class Home
{
    private Task OpenDialogAsync()
    {
        var options = new DialogOptions { CloseOnEscapeKey = true };

        return DialogService.ShowAsync<StudentLogin>("Simple Dialog", options);
    }
}