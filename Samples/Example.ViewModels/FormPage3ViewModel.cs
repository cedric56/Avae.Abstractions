using Avae.Services;
using Avae.ViewModels;

namespace Example.ViewModels;

public partial class FormPage3ViewModel(IDialogService dialog) 
    : INavigable
{
    public static string Title => "Go Back";

    int count = 0;
    public async Task<bool> CanNavigateAsync()
    {
        if (count <= 1)
        {
            await dialog.ShowOkAsync("This is a test, some values are required");
            count++;
            return false;
        }

        return true;
    }

    public Task OnNavigatedFrom(NavigableContext context)
    {
        return Task.CompletedTask;
    }

    public Task OnNavigatedTo(NavigableContext context)
    {
        return Task.CompletedTask;
    }
}
