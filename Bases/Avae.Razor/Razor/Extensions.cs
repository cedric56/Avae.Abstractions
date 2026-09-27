using Avae.Services;
using Avae.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using MudBlazor.Services;
using System.Diagnostics.CodeAnalysis;

namespace Avae.Razor;

internal interface IManagerReload
{
    bool Reload { get; }
}

class ManagerReload(TypeRazorProject typeRazorProject) : IManagerReload
{
    public bool Reload { get => typeRazorProject == TypeRazorProject.Server; }
}

public enum TypeRazorProject
{
    Server,
    Wasm
}

public static class Extensions
{
    class CircuitProvider(Action<IServiceProvider> initialize) : ICircuitProvider
    {
        public IServiceProvider Provider { get => null!; set => initialize(value); }
    }

    public static void RegisterViewFor<
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TComponent,
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TViewModel>(
    this IServiceCollection services,
    ServiceLifetime viewModelLifetime = ServiceLifetime.Transient,
    ServiceLifetime viewLifetime = ServiceLifetime.Transient,
    string? key = null)
    where TComponent : class where TViewModel : class
    => services.RegisterWithLifetime<ViewFor<TComponent, TViewModel>, TViewModel>(
        (sp) => new ViewFor<TComponent, TViewModel>(), key: key);

    public static void RegisterViewFor<
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TComponent,
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TViewModel>(
    this IServiceCollection services,
    Func<IServiceProvider, ViewFor<TComponent, TViewModel>> func,
    ServiceLifetime viewModelLifetime = ServiceLifetime.Transient,
    ServiceLifetime viewLifetime = ServiceLifetime.Transient,
    string? key = null)
    where TComponent : class where TViewModel : class
    => services.RegisterWithLifetime<ViewFor<TComponent, TViewModel>, TViewModel>(
        func, key: key);

    public static void RegisterViewFor<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TComponent,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TViewModel,
        TArg1>(
        this IServiceCollection services,
        Func<IServiceProvider, TArg1, ViewFor<TComponent, TViewModel>> func,
        ServiceLifetime viewModelLifetime = ServiceLifetime.Transient,
        ServiceLifetime viewLifetime = ServiceLifetime.Transient,
        string? key = null)
        where TComponent : class where TViewModel : class
        => services.RegisterWithLifetime<ViewFor<TComponent, TViewModel>, TViewModel, TArg1>(
            func, key: key);

    public static void UseAvae(this IServiceCollection services,
        TypeRazorProject typeRazorProject,
        NotificationPosition position = NotificationPosition.BottomLeft,
        int maxDispayments = 5,
        Action<IServiceProvider>? initialize = null)
    {
        var circuitProvider = new CircuitProvider(initialize ?? (sp => { }));

        services.AddMudServices(config =>
        {
            config.SnackbarConfiguration = new SnackbarConfiguration()
            {
                PositionClass = position switch
                {
                    NotificationPosition.TopLeft => Defaults.Classes.Position.TopLeft,
                    NotificationPosition.TopCenter => Defaults.Classes.Position.TopCenter,
                    NotificationPosition.TopRight => Defaults.Classes.Position.TopRight,
                    NotificationPosition.BottomLeft => Defaults.Classes.Position.BottomLeft,
                    NotificationPosition.BottomCenter => Defaults.Classes.Position.BottomCenter,
                    NotificationPosition.BottomRight => Defaults.Classes.Position.BottomRight,
                    _ => Defaults.Classes.Position.TopRight
                },
                MaxDisplayedSnackbars = maxDispayments
            };
        });
        services.AddSingleton<IManagerReload>(new ManagerReload(typeRazorProject));
        services.AddSingleton<ICircuitProvider>(circuitProvider);
        services.AddTransient<Router>();
        services.AddSingleton<Avae.Services.IDialogService, DialogService>();
        services.AddSingleton<IContentDialogService, ContentDialogService>();
        services.AddSingleton<ITaskDialogService, TaskDialogService>();
        services.AddSingleton<INotificationService, NotificationService>();
        services.AddSingleton<IRequestedThemeService, RequestThemeService>();
    }
}
