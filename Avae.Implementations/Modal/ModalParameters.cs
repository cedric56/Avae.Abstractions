using Avae.Abstractions;
using Avalonia.Controls;
using Avalonia.Platform;
using CommunityToolkit.Mvvm.Input;
using MsBox.Avalonia.Dto;
using MsBox.Avalonia.Models;
using System.Windows.Input;

namespace Avae.Implementations;

public class ModalButton : ButtonDefinition
{
    public ICommand? Command { get; set; }
}

public abstract class ModalParameters : MessageBoxCustomParams
{
    public IEnumerable<ModalButton> Definitions
    {
        get { return ButtonDefinitions.Cast<ModalButton>(); }
    }
    public UserControl? Content { get; set; }
}

public class ModalParameters<T, TResult> : ModalParameters
    where T : CloseableViewModelBase<TResult>
{  
    public ModalParameters(string icon, string buttons, T viewModel)
    {
        var type = typeof(T);
        var commands = type.GetProperties().Where(m => m.PropertyType == typeof(ICommand) || m.PropertyType == typeof(IAsyncRelayCommand) || m.PropertyType == typeof(RelayCommand)).ToList();
        var indexes = type.GetMethods().Where(m => m.CustomAttributes.Any(c => c.AttributeType == typeof(CloseableAttribute))).ToList();

        var definitions = new List<ButtonDefinition>();
        var names = buttons.Split(",").ToList();
        foreach (var name in names)
        {
            var prop = commands.FirstOrDefault(c => c.Name == "CloseCommand");
            var closeCommand = prop?.GetValue(viewModel) as ICommand;

            var index = names.IndexOf(name);
            var attr = indexes.FirstOrDefault(a => indexes.IndexOf(a) == index);
            if (attr != null)
            {
                prop = commands.ElementAt(index);
                closeCommand = prop.GetValue(viewModel) as ICommand;
            }

            var bd = new ModalButton
            {
                Command = closeCommand,
                Name = name,
                IsDefault = names.IndexOf(name) == 0,
                IsCancel = names.IndexOf(name) == names.Count - 1,
            };
            definitions.Add(bd);
        }
        ButtonDefinitions = definitions;

        try
        {
            WindowIcon = new WindowIcon(AssetLoader.Open(new Uri(icon)));
        }
        catch
        {

        }
        CloseOnClickAway = true;
        WindowStartupLocation = WindowStartupLocation.CenterOwner;
        CanResize = false;
        MaxWidth = 500;
        MaxHeight = 800;
        SizeToContent = SizeToContent.WidthAndHeight;
        ShowInCenter = true;
        Topmost = true;
    }
}
