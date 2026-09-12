# Avae.Abstractions

> Yet another navigation framework — but one built to run the *same* MVVM app across Avalonia (desktop/Linux), .NET MAUI (mobile/desktop), and Blazor (web), from a single shared ViewModel layer.

Avae is a set of small, composable libraries that let you write navigation, view-resolution, and data-access logic **once** and reuse it across every .NET UI stack. Instead of re-implementing routing and DI wiring per platform, you write against Avae's abstractions and plug in the platform-specific adapter.

--- 

## Why Avae?

Most cross-platform .NET UI frameworks stop at "shared business logic." Avae goes a step further and shares the **navigation and view-model orchestration layer** too:

- A **ViewModel** decides what page comes next.
- A platform-specific **View resolver** (Avalonia `IocContainer`, MAUI `Shell`/`FlyoutEx`, Blazor router) turns that decision into an actual on-screen page.
- Your `NavigableViewModelBase`, `Router`, and navigation state work identically whether the app is running as an Avalonia desktop app, a MAUI mobile app, or a Blazor page — only the rendering adapter changes.

---

## Repository layout

All library packages live under `Bases/`. Every target platform depends on `Avae.ViewModels` for its navigation/IoC contracts, then adds only the packages it needs.

| Project | Purpose |
|---|---|
| `Avae.ViewModels` | Core abstractions and base classes shared by every other package (`IIocContainer`, `IViewModelBase`, `IViewFor`, `NavigableViewModelBase`, `Router`, etc.) — the seed that everything else depends on. |
| `Avae.ViewModels.Tests` | Unit tests for the `Avae.ViewModels` layer. |
| `Avae.Core` | Small platform-agnostic helpers (async helpers, certificate handling, input validation). |
| `Avae.Essentials` | A cross-platform device/feature layer in the spirit of MAUI Essentials — geolocation, battery, clipboard, sensors, media picker, sharing, and more — usable from Avalonia, MAUI, or Blazor. |
| `Avae.Avalonia` | Avalonia-specific implementation: view resolution, navigation hosts, `IocContainer` wiring. |
| `Avae.Avalonia.Browser` | Avalonia running in the browser via WebAssembly. |
| `Avae.Maui` | .NET MAUI-specific implementation: `Shell`-based navigation and MAUI-flavored view resolution. |
| `Avae.Razor` | Blazor/Razor Components integration (built on MudBlazor) for running Avae ViewModels on the web. |
| `Avae.Notifications` | Cross-platform local/system notifications for Avalonia, MAUI, and Blazor. |
| `Avae.DAL` | Data-access-layer abstractions shared across storage providers. |
| `Avae.DAL.Sqlite` / `Avae.DAL.PostgreSQL` | Concrete `Avae.DAL` implementations for SQLite and PostgreSQL. |
| `Avae.DAL.SqlTableDependency` | Real-time SQL change notifications wired into the Avae data layer. |
| `Avae.DAL.SignalR` | Real-time client/server messaging integration. |
| `Avae.DAL.gRPC` / `Avae.DAL.gRPC.Client` | gRPC-based data-access server and client, built on MagicOnion. |
| `Avae.Server` / `Avae.Services` | Server-side hosting and service abstractions. |

### Examples

Every supported target has a runnable sample so you can see the same ViewModel layer hosted on each platform:

| Example | Platform |
|---|---|
| `Example.Desktop` | Avalonia desktop (Windows/Linux/macOS) |
| `Example.Windows` / `Example.macOS` | Platform-specific Avalonia packaging |
| `Example.Android` / `Example.iOS` | Avalonia/MAUI mobile |
| `Example.Maui` | .NET MAUI |
| `Example.BlazorApp` / `Example.BlazorAssembly` | Blazor Server / WebAssembly |
| `Example.Razor` | Razor Components |
| `Example.Browser` | Avalonia browser-hosted app (WebAssembly) |
| `Example.Hybrid` | Hybrid (MAUI Blazor Hybrid–style) app |
| `Example.Uno` | Uno Platform host |
| `Example.Server` | Backend/server host |
| `Example` / `Example.ViewModels` / `Example.Models` / `Example.DAL` | Shared code referenced by every sample above |
| `Example.ViewModels.Tests` | Tests for the shared example ViewModel layer |

---

## Core concepts

### ViewModels drive navigation, not views

```csharp
public abstract partial class NavigableViewModelBase : RouterViewModelBase, IViewModelBase
{
    public ObservableCollection<NavigableView> Navigables { get; }
    public NavigableView? SelectedNavigable { get; set; }
    public IViewFor CurrentView { get; set; }
    // ...
}
```

Each `NavigableView` declares a destination (`ViewModelType`, `DisplayName`, optional icon key) without knowing anything about Avalonia, MAUI, or Blazor. The platform adapter's `Router` resolves the actual page.

### One IoC container, per-platform view factories

```csharp
public interface IIocContainer
{
    void Register(string key, Func<IServiceProvider, object[], object> factory);
    void Register<TContextFor>() where TContextFor : IViewFor, new();
    void Register<TContextFor>(Func<IServiceProvider, NavigableContext, TContextFor> factory)
        where TContextFor : IViewFor;
    // + overloads for TContextFor with up to 5 constructor arguments
}
```

Views are registered by key (typically the ViewModel's type name) and resolved through a shared `NavigableContext`, so the same registration pattern works whether `TContextFor` is an Avalonia `UserControl`, a MAUI `ContentPage`, or a Razor component.

### Shared command infrastructure

`GoBack` / `GoForward` navigation commands (built on CommunityToolkit.Mvvm's `[RelayCommand]`) live in one shared base class and are inherited by every navigable and form ViewModel, so back/forward behavior is consistent across all hosted platforms.

---

## Getting started

> Clone the repo and open `Avae.Abstractions.sln` in Visual Studio / Rider / VS Code with the .NET MAUI and Avalonia workloads installed.

```bash
git clone https://github.com/cedric56/Avae.Abstractions.git
cd Avae.Abstractions
dotnet restore Avae.Abstractions.sln
```

Pick the example that matches your target platform and run it:

```bash
dotnet run --project Samples/Example.Desktop      # Avalonia desktop
dotnet run --project Samples/Example.Maui         # .NET MAUI
dotnet run --project Samples/Example.BlazorApp    # Blazor
```

Each example references `Example.ViewModels`, `Example.Models`, and `Example.DAL` — the shared layer you'd replace with your own app's equivalents.

---

## Status

This project is under active development. APIs may change between commits. Contributions, issues, and design feedback are welcome via [GitHub Issues](https://github.com/cedric56/Avae.Abstractions/issues).

## License

_Add your chosen license here (e.g. MIT) and drop a `LICENSE` file at the repo root — none is currently published._

## Contributing

Pull requests are welcome. If you're proposing a new platform adapter (e.g. WinUI), please open an issue first to discuss how it fits the `IIocContainer` / `Router` / `IViewFor` contracts in `Avae.ViewModels`.
