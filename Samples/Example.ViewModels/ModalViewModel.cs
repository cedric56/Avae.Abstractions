using Avae.Services;
using Avae.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Windows.Input;

namespace Example.ViewModels;

public static class ObservableValidatorExtensions
{
    public static string ValidateProperty(this ObservableValidator validator, string propertyName)
    {
        return string.Join(Environment.NewLine, validator.GetErrors("Message"));
    }
}

public partial class ModalViewModel(IDialogService dialogService) : ObservableValidator,
    ICloseableViewModel<string?>,
    IViewModelErrorInfo
{
    [ObservableProperty]
    [Required(ErrorMessage = "You have to enter a value.")]
    public partial string? Message { get; set; }

    public event EventHandler<string?>? CloseRequested;

    public string Error
    {
        get
        {
            return EntityValidator.Error(this) ?? string.Empty;
        }
    }

    public ICommand? CloseCommand { get; }

    public ObservableCollection<NamedCommand> Commands =>
        [
            new() { Command = ValidateCommand, Name = "Validate"},
            new() { Command = CancelCommand, Name="Cancel"}
        ];

    public string Title => "Modal";

    public string this[string columnName]
    {

        get
        {
            return EntityValidator.ValidateProperty(this, columnName) ?? string.Empty;
        }
    }

    [RelayCommand()]
    public async Task Validate()
    {
        if (await CanClose())
            await Close(Message!);
        else
            await dialogService.ShowOkAsync(Error, "Error");
    }

    [RelayCommand]
    public Task Cancel()
    {
        return Close(null);
    }

    protected Task<bool> CanClose()
    {
        return Task.FromResult(string.IsNullOrWhiteSpace(Error));
    }

    public Task Close(string? value)
    {
        CloseRequested?.Invoke(this, value);
        return Task.CompletedTask;
    }

    public void RaiseColumnErrorChanged(string name = "Item")
    {
        this.OnPropertyChanged(name);
    }
}
