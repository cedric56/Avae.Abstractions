
using Avae.Core;

namespace Avae.ViewModels;

/// <summary>
/// This class is used to represent a page in the application.
/// </summary>
/// <param name="viewModelType"></param>
/// <param name="displayName"></param>
/// <param name="path"></param>
public class NavigableView(Type viewModelType, string displayName, string? path = null)
{
    public Func<IViewModelBase, Task>? Launched { get; set; }
    public IViewModelBase? ViewModel { get; protected set; }
    public Type ViewModelType { get; } = viewModelType;
    public string DisplayName { get; } = displayName;
    public string? Path { get; } = path;

    public object? Icon
    {
        get
        {
            if (string.IsNullOrWhiteSpace(Path)) return null;
            return IconResolver.GetIcon(Path);
        }
    }
    public object? Source
    {
        get
        {
            if (string.IsNullOrWhiteSpace(Path)) return null;
            return IconResolver.GetSource(Path);
        }
    }
    public NavigableContext Context { get; set; } = new NavigableContext();

    public virtual Task OnLaunched(IViewModelBase viewModel)
    {
        if (Launched == null)
            return Task.CompletedTask;

        return Launched(viewModel);
    }
}

public class NavigableView<T> : NavigableView where T : IViewModelBase
{
    public new Func<T, Task>? Launched { get; set; }

    public NavigableView(string displayName, string? icon = null)
        : base(typeof(T), displayName, icon)
    {

    }

    public NavigableView(T viewModel, string displayName, string? icon = null)
        : base(typeof(T), displayName, icon)
    {
        ViewModel = viewModel;
    }

    public override Task OnLaunched(IViewModelBase viewModel)
    {
        if (Launched == null)
            return Task.CompletedTask;

        return Launched((T)viewModel);
    }
}
