namespace Avae.Services;

public enum RequestedTheme
{
    //
    // Summary:
    //     Default, unknown or unspecified theme.
    Default,
    //
    // Summary:
    //     Light theme.
    Light,
    //
    // Summary:
    //     Dark theme.
    Dark
}

public interface IRequestedThemeService
{
    void Request(RequestedTheme theme);
}
