using Avae.Abstractions;
using System.Collections.ObjectModel;
using UXDivers.Popups;
using UXDivers.Popups.Maui;

namespace Avae.Maui;

public partial class AvaePopupPage : PopupPage
{
    public class CommandIndex : NamedCommand
    {
        public required int Index { get; set; }
    }

    public AvaePopupPage(
        string title,
        ObservableCollection<NamedCommand> commands)
    {
        Title = title;
        Buttons = commands;

        InitializeComponent();

        if (Application.Current?.RequestedTheme == AppTheme.Light)
        {
            PopupBackground = Colors.White;
            Background = Color.FromArgb("#80B2B2B2");
        }
    }

    public string Title
    {
        get;set;
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

public partial class AvaePopupPage<TResult>(string title, ObservableCollection<NamedCommand> commands) : AvaePopupPage(title, commands), IPopupResultPage<TResult?>
{
    public TResult? Result { get; set; }

    public void SetResult(TResult? result)
    {
        Result = result;
    }
}