using System.ComponentModel;

namespace Avae.Maui.Themes;

public partial class ErrorStyles : ResourceDictionary
{
	public ErrorStyles()
	{
		InitializeComponent();
	}
}

public static class Validation
{
    public static readonly BindableProperty EnabledProperty =
        BindableProperty.CreateAttached(
            "Enabled", typeof(bool), typeof(Validation), false,
            propertyChanged: OnEnabledChanged);

    public static bool GetEnabled(BindableObject view) => (bool)view.GetValue(EnabledProperty);
    public static void SetEnabled(BindableObject view, bool value) => view.SetValue(EnabledProperty, value);

    static void OnEnabledChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is not View view) return;

        var existing = view.Behaviors.OfType<ValidationErrorsBehavior>().FirstOrDefault();

        if ((bool)newValue)
        {
            if (existing == null)
                view.Behaviors.Add(new ValidationErrorsBehavior());
        }
        else if (existing != null)
        {
            view.Behaviors.Remove(existing);
        }
    }
}

public enum ValidationPresentation
{
    Tooltip,
    InlineBorder
}

public class ValidationErrorsBehavior : Behavior<View>
{
    public static readonly BindableProperty PropertyNameProperty =
        BindableProperty.Create(nameof(PropertyName), typeof(string), typeof(ValidationErrorsBehavior));

    public static readonly BindableProperty PresentationProperty =
        BindableProperty.Create(nameof(Presentation), typeof(ValidationPresentation),
            typeof(ValidationErrorsBehavior), ValidationPresentation.Tooltip);

    public static readonly BindableProperty ErrorColorProperty =
        BindableProperty.Create(nameof(ErrorColor), typeof(Color),
            typeof(ValidationErrorsBehavior), Colors.Red);

    /// <summary>The property on the BindingContext (implementing INotifyDataErrorInfo) to watch.</summary>
    public string PropertyName
    {
        get => (string)GetValue(PropertyNameProperty);
        set => SetValue(PropertyNameProperty, value);
    }

    public ValidationPresentation Presentation
    {
        get => (ValidationPresentation)GetValue(PresentationProperty);
        set => SetValue(PresentationProperty, value);
    }

    public Color ErrorColor
    {
        get => (Color)GetValue(ErrorColorProperty);
        set => SetValue(ErrorColorProperty, value);
    }

    View? _view;
    INotifyDataErrorInfo? _errorSource;
    Color? _originalBorderColor;
    double _originalBorderWidth;

    protected override void OnAttachedTo(View bindable)
    {
        base.OnAttachedTo(bindable);
        _view = bindable;
        bindable.BindingContextChanged += OnBindingContextChanged;
        Hook(bindable.BindingContext);
    }

    protected override void OnDetachingFrom(View bindable)
    {
        base.OnDetachingFrom(bindable);
        bindable.BindingContextChanged -= OnBindingContextChanged;
        Unhook();
        _view = null;
    }

    void OnBindingContextChanged(object? sender, EventArgs e)
    {
        Unhook();
        Hook(_view?.BindingContext);
    }

    void Hook(object? context)
    {
        if (context is not INotifyDataErrorInfo errorInfo) return;

        _errorSource = errorInfo;
        _errorSource.ErrorsChanged += OnErrorsChanged;

        // Evaluate current state immediately (e.g. after initial validation)
        Refresh();
    }

    void Unhook()
    {
        if (_errorSource != null)
            _errorSource.ErrorsChanged -= OnErrorsChanged;
        _errorSource = null;
    }

    void OnErrorsChanged(object? sender, DataErrorsChangedEventArgs e)
    {
        // Only refresh if it's our property (or unspecified/batch update)
        if (string.IsNullOrEmpty(e.PropertyName) || e.PropertyName == PropertyName)
            Refresh();
    }

    void Refresh()
    {
        if (_view == null || _errorSource == null || string.IsNullOrEmpty(PropertyName))
            return;

        var errors = _errorSource.GetErrors(PropertyName)?
            .Cast<object>()
            .Select(x => x?.ToString())
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToList();

        var hasError = errors is { Count: > 0 };
        var message = hasError ? string.Join(Environment.NewLine, errors!) : null;

        MainThread.BeginInvokeOnMainThread(() => Apply(hasError, message));
    }

    void Apply(bool hasError, string? message)
    {
        if (_view == null) return;

        switch (Presentation)
        {
            case ValidationPresentation.Tooltip:
                ApplyTooltip(hasError, message);
                break;

            case ValidationPresentation.InlineBorder:
                ApplyInlineBorder(hasError, message);
                break;
        }

        VisualStateManager.GoToState(_view, hasError ? "Invalid" : "Valid");
    }

    void ApplyTooltip(bool hasError, string? message)
    {
        ToolTipProperties.SetText(_view!, hasError ? message : null);
    }

    void ApplyInlineBorder(bool hasError, string? message)
    {
        // Works on controls that expose these bindable props (Entry, Border, Frame, etc.)
        if (_view is Border border)
        {
            _originalBorderColor ??= (border.Stroke as SolidColorBrush)?.Color ?? Colors.Transparent;
            border.Stroke = hasError ? new SolidColorBrush(ErrorColor) : new SolidColorBrush(_originalBorderColor.Value);
        }
        else if (_view is Entry entry)
        {
            _originalBorderColor ??= entry.BorderColor;
            entry.BorderColor = hasError ? ErrorColor : _originalBorderColor.Value;
        }

        // Always keep a tooltip too as a fallback for the message text
        ToolTipProperties.SetText(_view!, hasError ? message : null);
    }
}