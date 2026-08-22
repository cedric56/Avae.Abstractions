using Avae.ViewModels;
using Avae.Core;
using Avae.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Windows.Input;

namespace Example.ViewModels;

public partial class ModalViewModel(IDialogService dialogService) : ObservableValidator, 
    ICloseableViewModel<string?>,
    IViewModelErrorInfo
{
    static ModalViewModel()
    {
        InputValidation<ModalViewModel>.Init();
    }

    [ObservableProperty]
    [Required(ErrorMessage = "You have to enter a value.")]
    public partial string? Message {  get; set; }

    public event EventHandler<string?>? CloseRequested;

    public string Error
    {
        get
        {
            return InputValidation<ModalViewModel>.Error(this);
        }
    }

    public ICommand? CloseCommand { get; }

    public ObservableCollection<NamedCommand> Commands =>
        [
            new() { Command = ValidateCommand, Name = "Valider"},
            new() { Command = CancelCommand, Name="Annuler"}
        ];

    public string Title => "Modal";

    public string this[string columnName]
    {

        get
        {
            return InputValidation<ModalViewModel>.Validate(this, columnName);
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

    public void RaiseErrorChanged()
    {
        this.OnPropertyChanged("Item");
    }
}
