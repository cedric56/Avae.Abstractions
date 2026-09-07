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

| Project | Purpose |
|---|---|
| `Avae.Abstractions` | Core interfaces and contracts shared by every other package (`IIocContainer`, `IViewModelBase`, `IViewFor`, etc.) — the seed that everything else depends on. |
| `Avae.Essentials` / `Avae.Everywhere` | Cross-cutting helpers usable from any platform (no UI dependency). |
| `Avae.Avalonia` | Avalonia-specific implementation: view resolution, `DrawerPage`/navigation hosts, `IocContainer` wiring. |
| `Avae.Maui` | .NET MAUI-specific implementation: `Shell`/`FlyoutEx` integration, MAUI-flavored view resolution. |
| `Avae.Razor` / `Avae.BlazorEssentials` | Blazor/Razor Components integration for running Avae ViewModels on the web. |
| `Avae.Browser` | Browser/WASM-hosted specifics. |
| `Avae.DAL` | Data-access-layer abstractions shared across storage providers. |
| `Avae.Sqlite` / `Avae.PostgreSQL` | Concrete `Avae.DAL` implementations for SQLite and PostgreSQL. |
| `Avae.SqlTableDependency` | Real-time SQL change notifications wired into the Avae data layer. |
| `Avae.SignalR` | Real-time client/server messaging integration. |
| `Avae.Server` / `Avae.Services` | Server-side hosting and service abstractions. |
| `Avae.MagicClient` / `Avae.MagicLayer` / `Avae.MagicServer` / `Avae.MagicServices` | Higher-level "Magic" convenience layer for wiring client, service, and server code together with less boilerplate. |

### Examples

Every supported target has a runnable sample so you can see the same ViewModel layer hosted on each platform:

| Example | Platform |
|---|---|
| `Example.Desktop` | Avalonia desktop (Windows/Linux/macOS) |
| `Example.Windows` / `Example.macOS` | Platform-specific Avalonia packaging |
| `Example.Android` / `Examples.iOS` | Avalonia/MAUI mobile |
| `Example.Maui` | .NET MAUI |
| `Example.BlazorApp` / `Example.BlazorAssembly` | Blazor Server / WebAssembly |
| `Example.Razor` | Razor Components |
| `Example.Browser` | Browser-hosted app |
| `Example.Hybrid` | Hybrid (MAUI Blazor Hybrid–style) app |
| `Example.Server` | Backend/server host |
| `Example.WebApplication` | ASP.NET Core web host |
| `Example.ViewModels` / `Example.Models` / `Example.DAL` | Shared code referenced by every sample above |

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
public class IocContainer : IIocContainer
{
    public void Register<TContextFor>() where TContextFor : IViewFor, new();
    public object GetView(string key, object[] context);
    // ...
}
```

Views are registered by key (typically the ViewModel's type name) and resolved through a shared `NavigableContext`, so the same registration pattern works whether `TContextFor` is an Avalonia `UserControl`, a MAUI `ContentPage`, or a Razor component.

### Shared command infrastructure

`GoBack` / `GoForward` navigation commands (built on CommunityToolkit.Mvvm's `[RelayCommand]`) live in one shared base class and are inherited by every navigable and form ViewModel, so back/forward behavior is consistent across all hosted platforms.

### Unified icon resolution

A small `IIconProvider` abstraction lets a single icon key (e.g. `"fa-solid fa-house"`) resolve to whatever each platform needs — a `Projektanker.Icons.Avalonia` glyph on Avalonia, a `MauiIcons` enum-backed glyph on MAUI, or a CSS class on Blazor — so icon names defined once in your view-model layer render correctly everywhere.

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
dotnet run --project Example.Desktop      # Avalonia desktop
dotnet run --project Example.Maui         # .NET MAUI
dotnet run --project Example.BlazorApp    # Blazor
```

Each example references `Example.ViewModels`, `Example.Models`, and `Example.DAL` — the shared layer you'd replace with your own app's equivalents.

---

## Status

This project is under active development. APIs may change between commits. Contributions, issues, and design feedback are welcome via [GitHub Issues](https://github.com/cedric56/Avae.Abstractions/issues).

## License

_Add your chosen license here (e.g. MIT) and drop a `LICENSE` file at the repo root — none is currently published._

## Contributing

Pull requests are welcome. If you're proposing a new platform adapter (e.g. WinUI, Uno), please open an issue first to discuss how it fits the `IIocContainer` / `Router` / `IViewFor` contracts in `Avae.Abstractions`.
