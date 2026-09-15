using Avae.ViewModels;
using Microsoft.Maui;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using System.Collections.ObjectModel;
using System.Linq;
using UXDivers.Popups;
using UXDivers.Popups.Maui;

namespace Avae.Abstractions;

public partial class TestPopupPage : PopupPage
{
    public class CommandIndex : NamedCommand
    {
        public required int Index { get; set; }
    }

    public TestPopupPage(
        string title,
        ObservableCollection<NamedCommand> commands)
    {
        Title = title;
        Buttons = commands;

        InitializeComponent();

        ControlTemplate = DeviceInfo.Platform switch
        {
            var p when p == DevicePlatform.Android => (ControlTemplate)Resources["AndroidTemplate"],
            var p when p == DevicePlatform.WinUI => (ControlTemplate)Resources["WindowsTemplate"],
            _ => (ControlTemplate)Resources["iOSTemplate"] // iOS + MacCatalyst
        };

        if (Application.Current?.RequestedTheme == AppTheme.Light)
        {
            PopupBackground = Colors.White;
            Background = Color.FromArgb("#80B2B2B2");
        }
    }

    public string Title
    {
        get; set;
    }

    public ObservableCollection<NamedCommand> Buttons
    {
        get;
        set;
    }

    public ColumnDefinitionCollection Definitions
    {
        get
        {
            return [.. Buttons?.Select(b => new ColumnDefinition(GridLength.Star)).ToArray() ?? []];
        }
    }

    public ObservableCollection<CommandIndex> Commands
    {
        get
        {
            return new ObservableCollection<CommandIndex>(
                Buttons?.Select(b => new CommandIndex()
                {
                    Name = b.Name,
                    Command = b.Command,
                    Index = Buttons.IndexOf(b)

                }) ?? []);
        }
    }
}

public partial class TestPopupPage<TResult>(string title, ObservableCollection<NamedCommand> commands) : TestPopupPage(title, commands), IPopupResultPage<TResult?>
{
    public TResult? Result { get; set; }

    public void SetResult(TResult? result)
    {
        Result = result;
    }
}