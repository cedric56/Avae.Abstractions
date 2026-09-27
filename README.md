# Avae.Abstractions

MAUI and Blazor/Razor **host adapters** for the Avae stack — the pieces that turn Avae's
platform-agnostic navigation core into an actual running app on those two hosts.

Write view-models, navigation, and shared services **once** in [`Avae.ViewModels`](https://github.com/cedric56/Avae.ViewModels),
then host that same layer on:

- **.NET MAUI** (mobile / desktop) — via `Avae.Maui`, this repo
- **Blazor** (Server / WebAssembly / components) — via `Avae.Razor`, this repo
- **Avalonia** (desktop / browser) — via `Avae.Avalonia`, a separate package/repo
- **Embedded Avalonia** (host Avalonia UI inside another process) — via `Avae.EmbeddedApp`, a separate package/repo

Avae is built on **`Microsoft.Extensions.DependencyInjection`**. There is no custom IoC container: views
and view-models are registered as keyed services and resolved through `IServiceProvider`.

> **Status:** active development. APIs may change between commits.

---

## What actually lives in *this* repo

This repo (`Avae.Abstractions`) currently contains **two** library projects and a set of runnable samples —
it is not the whole Avae stack. The navigation core, the Avalonia host, device APIs, notifications, and
data access all live in separate repositories/packages now and are pulled in here via NuGet.

```
Bases/
  Avae.Maui/     # .NET MAUI host: view resolution, dialogs/modals, notifications, theming
  Avae.Razor/    # Blazor/Razor host: MudBlazor-based dialogs/modals, components
Samples/
  Example.*/     # runnable hosts + shared Example.ViewModels/Example.Models
```

| Package | Where it lives | Role |
|---------|-----------------|------|
| **Avae.Maui** | `Bases/Avae.Maui` — *this repo* | MAUI host and view resolution |
| **Avae.Razor** | `Bases/Avae.Razor` — *this repo* | Blazor / Razor component integration (MudBlazor-based) |
| **Avae.ViewModels** | [separate repo](https://github.com/cedric56/Avae.ViewModels) | Core: `Router`, `NavigableView`, `IViewFor`, `INavigable`, registration helpers |
| **Avae.Avalonia** | separate package | Avalonia host, dialogs, modals, FluentAvalonia integration |
| **Avae.EmbeddedApp** | separate package | Embed Avalonia views in another host |
| **Avae.Essentials** | separate repo | Cross-platform device APIs (sensors, share, secure storage, …) |
| **Avae.Notifications** | separate package | Local / system notifications (feature-flagged) |
| **Avae.DAL** | separate package | Data access (Sqlite, PostgreSQL, SignalR, MagicOnion, … — feature-flagged) |
| **Avae.Services** | separate package (on [nuget.org](https://www.nuget.org/packages/Avae.Services)) | Shared service abstractions |

All of the above are pinned in `Directory.Packages.props` at `1.0.0-preview.1` and consumed as ordinary
`PackageReference`s from `Avae.Maui`/`Avae.Razor` — they are **not** `ProjectReference`s, so a fresh clone
of *only* this repo needs those packages to be resolvable from whatever NuGet feed(s) you have configured.

Optional CommunityToolkit.Mvvm helpers ship with **Avae.ViewModels** and are enabled via:

```xml
<AvaeFeatures>;CommunityToolkit;</AvaeFeatures>
```

---

## Core concepts

### 1. View-models drive navigation

```csharp
// Navigate by view-model type (creates the VM via DI)
var (view, vm) = await router.GoTo<HomeViewModel>();

// By runtime type
var view = await router.GoToType(typeof(HomeViewModel));
```

The `Router` (from `Avae.ViewModels`) keeps a back/forward history (view-model + view + `NavigableContext`)
and raises `CurrentViewModelChanged`.

### 2. Registration = `IServiceCollection`

```csharp
services.AddSingleton<Router>();

// View + ViewModel, optional key, optional lifetimes
services.Register<HomeView, HomeViewModel>();
services.RegisterWithLifetime<SettingsView, SettingsViewModel>(
    viewModelLifetime: ServiceLifetime.Transient,
    viewLifetime: ServiceLifetime.Transient,
    key: "Settings");
```

Views are resolved as keyed factories. View-models use the same mechanism (keyed by type or custom key).
Default key is `typeof(TViewModel).Name`.

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

## MAUI (`Avae.Maui`)

```csharp
builder.UseAvae();
// then register views/view-models on builder.Services
```

`Avae.Maui` provides `AvaeEntry`, a `TaskDialogService`/`ContentDialogService`/`ModalService`/`DialogService`
implementation backed by native platform dialogs (Android `AlertDialog`, WinUI `ContentDialog`, iOS
`UIAlertController`/form-sheet), a `RequestThemeService`, and a `NotificationService`.

> **Known gap:** `NotificationService.Show` and part of `ModalService` are currently implemented for
> **Windows only** (`#if WINDOWS`) — calling them on Android/iOS/MacCatalyst throws `NotImplementedException`
> at runtime today. If you're targeting mobile, don't rely on these two yet.

See `Samples/Example.Maui` for full wiring.

## Blazor / Razor (`Avae.Razor`)

```csharp
services.UseAvae(/* … */);
```

`Avae.Razor` provides MudBlazor-based `ContentDialog`/`TaskDialog` components and matching services.

See `Samples/Example.BlazorApp`, `Example.BlazorAssembly`, and `Example.Razor` for full wiring.

---

## Feature flags (`AvaeFeatures`)

Some packages ship optional backends as **source/content inside one package**. Opt in from the consuming
`.csproj`:

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
| `Example.Server` | MagicOnion / server |
| `Example.ViewModels` / `Example.Models` | **Shared** app layer used by the hosts above |
| `Example.ViewModels.Tests` | Tests for the shared example ViewModel layer |

Shared navigation logic lives under `Example.ViewModels` — that is the layer Avae is designed to keep portable.

---

## Getting started

```bash
git clone https://github.com/cedric56/Avae.Abstractions.git
cd Avae.Abstractions
dotnet restore Avae.Abstractions.sln
```

Requirements: .NET SDK matching the solution TFMs (**net11.0** / MAUI multi-targets), plus the MAUI workload,
and NuGet access to the `Avae.*` preview packages listed above (`Avae.ViewModels`, `Avae.Services`, etc. —
not vendored into this repo).

> **Note:** the repo ships a `nuget..config` file (double dot) intended to map Avalonia nightly packages to
> a separate feed. NuGet only recognizes the exact filename `nuget.config` / `NuGet.Config`, so this file is
> currently **silently ignored**. If a restore fails looking for an Avalonia-nightly-only version, rename it
> to `nuget.config`.

```bash
dotnet run --project Samples/Example.Maui
dotnet run --project Samples/Example.BlazorApp
```

### Local development tip

Because `Avae.ViewModels`, `Avae.Avalonia`, `Avae.Essentials`, `Avae.Notifications`, `Avae.DAL`, and
`Avae.Services` are no longer part of this solution, iterating on both this repo and one of those at the
same time means either: (a) publishing a local preview build of the other repo to a local NuGet feed, or
(b) temporarily swapping the relevant `PackageReference` in `Bases/Avae.Maui/Avae.Maui.csproj` /
`Bases/Avae.Razor/Avae.Razor.csproj` for a `ProjectReference` to your local checkout of that repo.

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

Issues and PRs are welcome. For a new UI adapter, open an issue first and align with the `Router` /
`IViewFor` / registration contracts in [**Avae.ViewModels**](https://github.com/cedric56/Avae.ViewModels).
