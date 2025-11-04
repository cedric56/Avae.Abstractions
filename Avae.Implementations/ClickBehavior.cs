using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;
using System.Windows.Input;

namespace Avae.Implementations;

public class ClickBehavior : AvaloniaObject
{
    static ClickBehavior()
    {
        CommandProperty.Changed.AddClassHandler<Interactive>(HandleCommandChanged);
    }

    /// <summary>
    /// Identifies the <seealso cref="CommandProperty"/> avalonia attached property.
    /// </summary>
    /// <value>Provide an <see cref="ICommand"/> derived object or binding.</value>
    public static readonly AttachedProperty<ICommand> CommandProperty = AvaloniaProperty.RegisterAttached<ClickBehavior, Interactive, ICommand>(
        "Command", default, false, BindingMode.OneTime);

    /// <summary>
    /// Identifies the <seealso cref="CommandParameterProperty"/> avalonia attached property.
    /// Use this as the parameter for the <see cref="CommandProperty"/>.
    /// </summary>
    /// <value>Any value of type <see cref="object"/>.</value>
    public static readonly AttachedProperty<object> CommandParameterProperty = AvaloniaProperty.RegisterAttached<ClickBehavior, Interactive, object>(
        "CommandParameter", default, false, BindingMode.OneWay, null);


    /// <summary>
    /// <see cref="CommandProperty"/> changed event handler.
    /// </summary>
    private static void HandleCommandChanged(Interactive interactElem, AvaloniaPropertyChangedEventArgs args)
    {
        if (args.NewValue is ICommand commandValue)
        {
            // Add non-null value
            interactElem.AddHandler(Button.ClickEvent, Handler);
        }
        else
        {
            // remove prev value
            interactElem.RemoveHandler(Button.ClickEvent, Handler);
        }
        // local handler fcn
        void Handler(Button sender, RoutedEventArgs e)
        {
            if (sender is Interactive interactElem)
            {
                // This is how we get the parameter off of the gui element.
                object commandParameter = interactElem.GetValue(CommandParameterProperty);
                ICommand commandValue = interactElem.GetValue(CommandProperty);
                if (commandValue?.CanExecute(commandParameter) == true)
                {
                    commandValue.Execute(commandParameter);
                }
            }
        }
    }


    /// <summary>
    /// Accessor for Attached property <see cref="CommandProperty"/>.
    /// </summary>
    public static void SetCommand(AvaloniaObject element, ICommand commandValue)
    {
        element.SetValue(CommandProperty, commandValue);
    }

    /// <summary>
    /// Accessor for Attached property <see cref="CommandProperty"/>.
    /// </summary>
    public static ICommand GetCommand(AvaloniaObject element)
    {
        return element.GetValue(CommandProperty);
    }

    /// <summary>
    /// Accessor for Attached property <see cref="CommandParameterProperty"/>.
    /// </summary>
    public static void SetCommandParameter(AvaloniaObject element, object parameter)
    {
        element.SetValue(CommandParameterProperty, parameter);
    }

    /// <summary>
    /// Accessor for Attached property <see cref="CommandParameterProperty"/>.
    /// </summary>
    public static object GetCommandParameter(AvaloniaObject element)
    {
        return element.GetValue(CommandParameterProperty);
    }
}
