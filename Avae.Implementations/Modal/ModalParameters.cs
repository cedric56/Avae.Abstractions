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
    where T : CloseableViewModelBase<TResult>
{  
    public ModalParameters(string icon, string buttons, T viewModel)
    {
        var type = typeof(T);
        var commands = type.GetProperties().Where(m => typeof(ICommand).IsAssignableFrom(m.PropertyType)).ToList();
        var prop = commands.FirstOrDefault(c => c.Name == "CloseCommand");
        var closeCommand = prop?.GetValue(viewModel) as ICommand;
        var indexes = type.GetMethods().Where(m => m.CustomAttributes.Any(c => c.AttributeType == typeof(CloseableAttribute))).ToList();
        var definitions = new List<ButtonDefinition>();
        var names = buttons.Split(",").ToList();
        foreach (var name in names)
        {
            var index = names.IndexOf(name);
            var attr = indexes.FirstOrDefault(a => indexes.IndexOf(a) == index);
            if (attr != null)
            {
                prop = commands.Count() > index ? commands.ElementAt(index) : commands.LastOrDefault();
                closeCommand = prop?.GetValue(viewModel) as ICommand;
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
            try
            {
                WindowIcon = new WindowIcon(AssetLoader.Open(new Uri(icon)));
            }
            catch
            {

            }
        }
    }
}
