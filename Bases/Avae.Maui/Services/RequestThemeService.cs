using Avae.Services;

namespace Avae.Maui.Services;

internal class RequestThemeService : IRequestedThemeService
{
    /// <summary>
    /// Applies the specified theme to the application by setting <see cref="Application.UserAppTheme"/>.
    /// </summary>
    /// <param name="theme">The requested theme.</param>
    public void Request(RequestedTheme theme)
    {
        Application.Current?.UserAppTheme
            = theme switch
            {
                RequestedTheme.Light => AppTheme.Light,
                RequestedTheme.Dark => AppTheme.Dark,
                _ => AppTheme.Unspecified,
            };
    }
}
