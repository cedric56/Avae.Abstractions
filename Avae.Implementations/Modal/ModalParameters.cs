using Avae.Abstractions;
using Avalonia.Controls;
using Avalonia.Platform;
using MsBox.Avalonia.Dto;
using MsBox.Avalonia.Models;
using System.Windows.Input;

namespace Avae.Implementations;

internal class ModalButton : ButtonDefinition
{
    public ICommand? Command { get; set; }
}

internal abstract class ModalParameters : MessageBoxCustomParams
{
    public IEnumerable<ModalButton> Definitions
    {
        get { return ButtonDefinitions.Cast<ModalButton>(); }
    }
    public UserControl? Content { get; set; }
}

internal class ModalParameters<T, TResult> : ModalParameters
    where T : ICloseableViewModel<TResult>
{
    public ModalParameters(string icon, T viewModel)
    {
        var definitions = new List<ButtonDefinition>();
        foreach (var command in viewModel.Commands)
        {
            var bd = new ModalButton
            {
                Command = command.Command,
                Name = command.Name,
                IsDefault = viewModel.Commands.IndexOf(command) == 0,
                IsCancel = viewModel.Commands.IndexOf(command) == viewModel.Commands.Count - 1,
            };
            definitions.Add(bd);
        }
        ButtonDefinitions = definitions;
        CloseOnClickAway = true;
        WindowStartupLocation = WindowStartupLocation.CenterOwner;
        CanResize = false;
        MaxWidth = 500;
        MaxHeight = 800;
        SizeToContent = SizeToContent.WidthAndHeight;
        ShowInCenter = true;
        Topmost = true;

        if (!string.IsNullOrWhiteSpace(icon))
        {
            var uri = new Uri(icon);
            if (AssetLoader.Exists(uri))
                WindowIcon = new WindowIcon(AssetLoader.Open(uri));
        }
    }
}
