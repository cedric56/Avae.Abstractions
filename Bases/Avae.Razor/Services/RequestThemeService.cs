using Avae.Services;

namespace Avae.Razor.Services;

internal class RequestThemeService : IRequestedThemeService
{
    public EventHandler<RequestedTheme>? RequestedThemeChanged;
    public bool IsDarkMode { get; set; } = true;

    public void Request(RequestedTheme theme)
    {
        IsDarkMode = theme == RequestedTheme.Dark;
        RequestedThemeChanged?.Invoke(this, theme);
    }
}
