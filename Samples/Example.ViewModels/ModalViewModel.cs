using Avae.Services;
using Avae.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Windows.Input;

namespace Example.ViewModels;

public partial class ModalViewModel(IDialogService dialogService) : ObservableValidator,
    ICloseableViewModel<string?>
{
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "You have to enter a value.")]
    public partial string? Message { get; set; }

    public event EventHandler<string?>? CloseRequested;

    public ICommand? CloseCommand { get; }

    public ObservableCollection<NamedCommand> Commands =>
        [
            new() { Command = ValidateCommand, Name = "Validate"},
            new() { Command = CancelCommand, Name="Cancel"}
        ];

    public string Title => "Modal";

    [RelayCommand()]
    public async Task Validate()
    {
        if (await CanClose())
            await Close(Message!);
        else
            await dialogService.ShowOkAsync(EntityValidator.Error(this) ?? string.Empty, "Error");
    }

    [RelayCommand]
    public Task Cancel()
    {
        return Close(null);
    }

    protected Task<bool> CanClose()
    {
        return Task.FromResult(string.IsNullOrWhiteSpace(EntityValidator.Error(this)));
    }

    public Task Close(string? value)
    {
        CloseRequested?.Invoke(this, value);
        return Task.CompletedTask;
    }
}
