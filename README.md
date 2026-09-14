# Avae.Abstractions

> Yet another navigation framework — but one built to run the *same* MVVM app across Avalonia (desktop/Linux), .NET MAUI (mobile/desktop), and Blazor (web), from a single shared ViewModel layer.

Avae lets you write navigation, view-resolution, and data-access logic **once** and reuse it across every .NET UI stack. Instead of re-implementing routing and DI wiring per platform, you write against Avae's abstractions and opt in to the platform-specific code you need through a single MSBuild property — `AvaeFeatures` — rather than pulling in a different NuGet package per platform.

--- 

## Why Avae?

Most cross-platform .NET UI frameworks stop at "shared business logic." Avae goes a step further and shares the **navigation and view-model orchestration layer** too:

- A **ViewModel** decides what page comes next.
- A platform-specific **View resolver** (Avalonia `IocContainer`, MAUI `Shell`/`FlyoutEx`, Blazor router) turns that decision into an actual on-screen page.
- Your `NavigableViewModelBase`, `Router`, and navigation state work identically whether the app is running as an Avalonia desktop app, a MAUI mobile app, or a Blazor page — only the rendering adapter changes.

---

## Repository layout

Library code lives under `Bases/`. Rather than one NuGet package per platform, Avae ships a small number of **feature-flagged packages** — each one contains source for every platform it supports, and a consumer opts in to just the parts it needs by setting `<AvaeFeatures>` in its own `.csproj`.

| Project | Purpose |
|---|---|
| `Avae.ViewModels` | Core abstractions and base classes shared by everything else (`IIocContainer`, `IViewModelBase`, `IViewFor`, `NavigableViewModelBase`, `Router`, etc.) — the seed every other package depends on. |
| `Avae.ViewModels.Tests` | Unit tests for the `Avae.ViewModels` layer. |
| `Avae.Essentials` | A cross-platform device/feature layer in the spirit of MAUI Essentials — geolocation, battery, clipboard, sensors, media picker, sharing, and more — usable from Avalonia, MAUI, or Blazor. |
| `Avae.Abstractions` | The UI-adapter package. Feature-gated via `AvaeFeatures`: `Avalonia` (view resolution, `IocContainer`, navigation hosts, FluentAvalonia/MessageBox integration), `Maui` (`AvaeEntry`, popups, Shell-flavored view resolution), `Razor` (MudBlazor-based Razor Components integration), `Embedded` (hosting an Avalonia view inside another host via `EmbeddedAvalonia`). |
| `Avae.Notifications` | Cross-platform local/system notifications. Feature-gated: `BlazorNotifications`, `MauiNotifications` (Avalonia notification support is unconditional). |
| `Avae.DAL` | Data-access layer. Feature-gated: `Sqlite`, `PostgreSQL`, `SqlTableDependency` (real-time SQL change notifications), `SignalR`, `MagicOnion`/`MagicClient`/`MagicServer` (gRPC via MagicOnion, split into shared/client/server pieces). |
| `Avae.Services` | Service abstractions and DI wiring shared across platforms. |

### The `AvaeFeatures` pattern

Instead of referencing `Avae.DAL.Sqlite`, `Avae.DAL.PostgreSQL`, etc. as separate packages, every consumer references the same `Avae.DAL` package and lists which pieces it wants:

```xml
<PropertyGroup>
  <AvaeFeatures>Sqlite;SignalR</AvaeFeatures>
</PropertyGroup>

<ItemGroup>
  <PackageReference Include="Avae.DAL" />
</ItemGroup>
```

Each package's `build`/`buildTransitive` `.targets` file reads `$(AvaeFeatures)` and conditionally adds the matching `PackageReference`s and source files — so a Sqlite-only consumer never pulls in Npgsql, SignalR, or MagicOnion dependencies it doesn't need. The same mechanism is used for platform selection in `Avae.Abstractions` (`Avalonia` / `Maui` / `Razor` / `Embedded`) and `Avae.Notifications` (`BlazorNotifications` / `MauiNotifications`).

### Examples

Every supported target has a runnable sample so you can see the same ViewModel layer hosted on each platform:

| Example | Platform | `AvaeFeatures` |
|---|---|---|
| `Example` | Avalonia desktop (Windows/Linux/macOS) | `Avalonia` |
| `Example.Windows` / `Example.macOS` / `Example.Android` / `Example.iOS` | Platform-specific Avalonia packaging | `Avalonia` |
| `Example.Desktop` | Avalonia desktop host | `Avalonia` |
| `Example.Browser` | Avalonia running in the browser (WebAssembly) | `Avalonia` |
| `Example.Maui` | .NET MAUI | `Maui;MauiNotifications` |
| `Example.Razor` | Razor Class Library / Components | `Razor` |
| `Example.BlazorApp` / `Example.BlazorAssembly` | Blazor Server / WebAssembly | `Razor` |
| `Example.Hybrid` | MAUI Blazor Hybrid | `Maui` |
| `Example.Uno` | Uno Platform host | n/a (Uno's own `UnoFeatures`, unrelated to Avae's) |
| `Example.Server` | Backend/server host, MagicOnion gRPC server | `MagicServer` |
| `Example.ViewModels` / `Example.Models` / `Example.DAL` | Shared code referenced by every sample above | — |
| `Example.ViewModels.Tests` | Tests for the shared example ViewModel layer | — |

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
    void Register<T>(Func<IServiceProvider, NavigableContext, object> factory);
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

### Adding Avae to your own project

Reference the packages you need and declare which pieces of them to activate via `AvaeFeatures`:

```xml
<PropertyGroup>
  <!-- e.g. for an Avalonia desktop app -->
  <AvaeFeatures>Avalonia</AvaeFeatures>
</PropertyGroup>

<ItemGroup>
  <PackageReference Include="Avae.ViewModels" />
  <PackageReference Include="Avae.Abstractions" />
  <PackageReference Include="Avae.Notifications" />
</ItemGroup>
```

> **Note:** if you're iterating on Avae itself rather than just consuming it, prefer `ProjectReference`s to the `Bases/` projects over `PackageReference` during local development — NuGet's package cache doesn't pick up source changes until you `dotnet pack` a new version and clear the old one from `~/.nuget/packages`.

---

## Status

This project is under active development. APIs may change between commits. Contributions, issues, and design feedback are welcome via [GitHub Issues](https://github.com/cedric56/Avae.Abstractions/issues).

## License

_Add your chosen license here (e.g. MIT) and drop a `LICENSE` file at the repo root — none is currently published._

## Contributing

Pull requests are welcome. If you're proposing a new platform adapter (e.g. WinUI), please open an issue first to discuss how it fits the `IIocContainer` / `Router` / `IViewFor` contracts in `Avae.ViewModels`.
