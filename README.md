# Avae

Cross-platform **MVVM navigation and app infrastructure** for .NET.

Write view-models, navigation, and shared services **once**, then host the same layer on:

- **Avalonia** (desktop / browser)
- **.NET MAUI** (mobile / desktop)
- **Blazor** (Server / WebAssembly / components)
- **Embedded Avalonia** (host Avalonia UI inside another process)

Avae is built on **`Microsoft.Extensions.DependencyInjection`**. There is no custom IoC container: views and view-models are registered as keyed services and resolved through `IServiceProvider`.

> **Status:** active development. APIs may change between commits.

---

## Packages

| Package | Role |
|---------|------|
| **Avae.ViewModels** | Core: `Router`, `NavigableView`, `IViewFor`, `INavigable`, registration helpers |
| **Avae.Avalonia** | Avalonia host, dialogs, modals, FluentAvalonia integration |
| **Avae.Maui** | MAUI host and view resolution |
| **Avae.Razor** | Blazor / Razor component integration |
| **Avae.Embedded** | Embed Avalonia views in another host |
| **Avae.Essentials** | Cross-platform device APIs (sensors, share, secure storage, …) |
| **Avae.Notifications** | Local / system notifications (feature-flagged) |
| **Avae.DAL** | Data access (Sqlite, PostgreSQL, SignalR, MagicOnion, … — feature-flagged) |
| **Avae.Services** | Shared service abstractions (e.g. broker) |

Optional CommunityToolkit.Mvvm helpers ship with **Avae.ViewModels** and are enabled via:

```xml
<AvaeFeatures>;CommunityToolkit;</AvaeFeatures>
```

---

## Core concepts

### 1. View-models drive navigation

```csharp
// Navigate by view-model type (key defaults to typeof(T).Name)
var view = await router.GoTo<HomeViewModel>(out var vm);

// Optional custom registration key
var view = await router.GoToType(typeof(HomeViewModel), key: "Home");
```

The `Router` keeps a back/forward history (view-model + view + `NavigableContext`) and raises `CurrentViewModelChanged`.

### 2. Registration = `IServiceCollection`

```csharp
services.AddTransient<Router>();

// View + ViewModel, optional key, optional lifetimes
services.Register<HomeView, HomeViewModel>();
services.RegisterWithLifetime<SettingsView, SettingsViewModel>(
    viewModelLifetime: ServiceLifetime.Transient,
    viewLifetime: ServiceLifetime.Transient,
    key: "Settings");
```

Views are resolved as keyed factories. View-models use the same mechanism (keyed by type or custom key).

### 3. Lifecycle (`INavigable`) — optional

```csharp
public interface INavigable
{
    Task<bool> CanNavigateAsync();           // return false to cancel
    Task OnNavigatedTo(NavigableContext context);
    Task OnNavigatedFrom(NavigableContext context);
}
```

Implemented by a view-model and/or a view. The router calls these during `GoTo`, `BackAsync`, and `ForwardAsync`.

### 4. Navigation context

```csharp
var context = NavigableContext.Create()
    .WithKey("Home")
    .WithViewModelParameters(userId)
    .WithViewParameters(someUiArg);
```

---

## Minimal usage (any UI stack)

### View-model (lightweight — no base class required)

```csharp
using CommunityToolkit.Mvvm.ComponentModel;
using Avae.ViewModels;

public partial class MainViewModel(Router router) : ObservableObject
{
    [ObservableProperty] private IViewFor? currentView;

    public ObservableCollection<NavigableView> Navigables { get; } =
    [
        new NavigableView<HomeViewModel>("Home", "fa-solid fa-house"),
        new NavigableView<SettingsViewModel>("Settings", "fa-solid fa-gear"),
    ];

    private NavigableView? _selected;
    public NavigableView? SelectedNavigable
    {
        get => _selected;
        set
        {
            if (SetProperty(ref _selected, value) && value is not null)
                _ = NavigateAsync(value);
        }
    }

    private async Task NavigateAsync(NavigableView item)
    {
        CurrentView = await router.GoToType(item.ViewModelType, context: item.Context);
    }
}
```

### XAML sketch (Avalonia)

```xml
<ContentControl Content="{Binding CurrentView}" />
<!-- menu: ItemsSource=Navigables, SelectedItem=SelectedNavigable -->
```

### Optional bases

- `NavigableViewModelBase` / `RouterViewModelBase` — menu shell, cache, back/forward wiring
- `Community/NavigableViewModel` — same + CommunityToolkit `[RelayCommand]` / `INotifyPropertyChanged`
  (enable with `AvaeFeatures` containing `CommunityToolkit`)

---

## Avalonia host

```csharp
using Avae.Avalonia;

public static AppBuilder BuildAvaloniaApp()
    => AvaeBuilder.CreateAvaloniaApp<App>(
        icon: null,
        isFluent: true,
        appFactory: sp => new App(sp),
        configureExternalServices: services =>
        {
            services.Register<HomeView, HomeViewModel>();
            services.Register<MainView, MainViewModel>();
            // …
        });
```

`CreateAvaloniaApp` registers `Router`, dialog/notification services, builds the `IServiceProvider`, and disposes it when the main window closes.

---

## MAUI / Blazor

```csharp
// MAUI
builder.UseAvae();
// then register views/view-models on builder.Services

// Blazor / Razor
services.UseAvae(/* … */);
```

See `Samples/Example.Maui`, `Example.BlazorApp`, and `Example.Razor` for full wiring.

---

## Feature flags (`AvaeFeatures`)

Some packages ship optional backends as **source/content inside one package**. Opt in from the consuming `.csproj`:

```xml
<PropertyGroup>
  <AvaeFeatures>;Sqlite;SignalR;CommunityToolkit;</AvaeFeatures>
</PropertyGroup>
```

Examples:

| Feature | Package |
|---------|---------|
| `CommunityToolkit` | Avae.ViewModels |
| `Sqlite`, `PostgreSQL`, `SignalR`, `MagicServer`, … | Avae.DAL |
| `BlazorNotifications`, `MauiNotifications` | Avae.Notifications |

Only the features you list pull the related dependencies.

---

## Samples

| Project | Target |
|---------|--------|
| `Example` / `Example.Desktop` | Avalonia desktop |
| `Example.Browser` | Avalonia WebAssembly |
| `Example.Windows` / `macOS` / `Android` / `iOS` | Platform packaging |
| `Example.Maui` | .NET MAUI |
| `Example.Hybrid` | MAUI Blazor Hybrid |
| `Example.BlazorApp` / `Example.BlazorAssembly` | Blazor |
| `Example.Razor` | Razor class library |
| `Example.Uno` | Uno Platform host |
| `Example.Server` | MagicOnion / server |
| `Example.ViewModels` / `Models` / `DAL` | **Shared** app layer used by the hosts above |

Shared navigation logic lives under `Example.ViewModels` — that is the layer Avae is designed to keep portable.

---

## Getting started

```bash
git clone https://github.com/cedric56/Avae.Abstractions.git
cd Avae.Abstractions
dotnet restore Avae.Abstractions.sln
```

Requirements: .NET SDK matching the solution TFMs (projects target **net11.0** / multi-target MAUI), plus Avalonia and/or MAUI workloads as needed.

```bash
dotnet run --project Samples/Example.Desktop
dotnet run --project Samples/Example.Maui
dotnet run --project Samples/Example.BlazorApp
```

### Local development tip

While iterating on Avae itself, prefer `ProjectReference` to `Bases/*` over NuGet `PackageReference`. Package caches do not pick up source changes until you pack and clear the local feed.

---

## Solution layout

```
Bases/
  Avae.ViewModels/          # navigation core
  Avae.ViewModels.Tests/
  Avae.Avalonia/
  Avae.Maui/
  Avae.Razor/
  Avae.Embedded/
  Avae.Essentials/
  Avae.Notifications/
  Avae.DAL/
  Avae.Services/
Samples/
  Example.*/                # runnable hosts + shared Example.ViewModels
```

---

## Design notes

- **One DI stack:** `IServiceCollection` / `IServiceProvider` only.
- **View-model first:** navigate to a type (or instance); the view is resolved by key.
- **Async navigation:** `GoTo*`, `BackAsync`, `ForwardAsync` return `Task`; cancellation via `INavigable.CanNavigateAsync`.
- **Optional complexity:** use plain `ObservableObject` + `Router`, or the navigable base classes when you want a menu shell and history helpers.

---

## License

MIT — see [LICENSE](LICENSE) and [THIRD-PARTY-NOTICES.txt](THIRD-PARTY-NOTICES.txt).

---

## Contributing

Issues and PRs are welcome. For a new UI adapter, open an issue first and align with the `Router` / `IViewFor` / registration contracts in **Avae.ViewModels**.
