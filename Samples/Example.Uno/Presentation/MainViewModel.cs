using CommunityToolkit.Mvvm.ComponentModel;
using Example.Uno.Models;
using Microsoft.Extensions.Options;

namespace Example.Uno.Presentation;

public partial class MainViewModel : ObservableObject
{

    public MainViewModel(
        IOptions<AppConfig> appInfo)
    {
        Title = "Main";
        //Title += $" - {localizer["ApplicationName"]}";
        Title += $" - {appInfo?.Value?.Environment}";
    }
    public string? Title { get; }


}
