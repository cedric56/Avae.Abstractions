namespace Avae.Services
{
    public enum RequestedTheme
    {
        //
        // Summary:
        //     Default, unknown or unspecified theme.
        Unspecified,
        //
        // Summary:
        //     Light theme.
        Light,
        //
        // Summary:
        //     Dark theme.
        Dark
    }

    public interface IRequestedTheme
    {
        void Request(RequestedTheme theme);
    }
}
