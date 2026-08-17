using Avae.Essentials.Core;
using Avalonia.Controls.Maui.Essentials;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Accessibility;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.ApplicationModel.Communication;
using Microsoft.Maui.ApplicationModel.DataTransfer;
using Microsoft.Maui.Authentication;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.Media;
using Microsoft.Maui.Networking;
using Microsoft.Maui.Storage;

namespace Avae.Essentials.Avalonia;

public static class Extensions
{
    public static void UseAvaeEssentials(this IServiceCollection services)
    {
        var platformProvider = new AvaeTopLevelStateManager();
        var screenshot = new AvaloniaScreenshot(platformProvider);
        var filepicker = (IFilePicker)AvaloniaDefaults.CreateAvaloniaFilePicker(platformProvider);
        var mediapicker = (IMediaPicker)AvaloniaDefaults.CreateAvaloniaMediaPicker(platformProvider);
        var hapticFeedback = new AvaloniaHapticFeedback();
        var preferences = new AvaloniaPreferences();
        var fileSystem = new AvaloniaFileSystem();
        var webAuthenticator = (IWebAuthenticator)AvaloniaDefaults.CreateAvaloniaWebAuthenticator(platformProvider);

#if MACOS
        services.SetDefaults(
            null!,
            null!,
            (IAppInfo)MacosDefaults.CreateAppInfo(),
            null!,
            (IBattery)MacosDefaults.CreateBattery(),
            (IBrowser)MacosDefaults.CreateBrowser(),
            (IClipboard)MacosDefaults.CreateClipboard(),
            null!,
            (IConnectivity)MacosDefaults.CreateConnectivity(),
            null!,
            (IDeviceDisplay)MacosDefaults.CreateDeviceDisplay(),
            (IDeviceInfo)MacosDefaults.CreateDeviceInfo(),
            (IEmail)MacosDefaults.CreateEmail(),
            filepicker,
            fileSystem,
            (IFlashlight)MacosDefaults.CreateFlashlight(),
            null!,
            (IGeolocation)MacosDefaults.CreateGeolocation(),
            null!,
            hapticFeedback,
            (ILauncher)MacosDefaults.CreateLauncher(),
            null!,
            (IMap)MacosDefaults.CreateMap(),
            mediapicker,
            null!,
            (IPhoneDialer)MacosDefaults.CreatePhoneDialer(),
            preferences,
            screenshot,
            () => (ISecureStorage)MacosDefaults.CreateSecureStorage(),
            (ISemanticScreenReader)MacosDefaults.CreateSemanticScreenReader(),
            (IShare)MacosDefaults.CreateShare(),
            (ISms)MacosDefaults.CreateSms(),
            (ITextToSpeech)MacosDefaults.CreateTextToSpeech(),
            (IVibration)MacosDefaults.CreateVibration(),
            webAuthenticator);

#elif WINDOWS_OS && !IOS && !ANDROID && !BROWSER

        services.SetDefaults(
            OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10240, 0) ? new Avae.Essentials.Avalonia.AccelerometerImplementation() : null!,
            OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10586, 0) ? new Avae.Essentials.Avalonia.AppActionsImplementation() : null!,
            OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10240, 0) ? new Avae.Essentials.Avalonia.AvaeAppInfo() : null!,
            OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10240, 0) ? new Avae.Essentials.Avalonia.BarometerImplementation() : null!,
            OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10240, 0) ? new Avae.Essentials.Avalonia.BatteryImplementation() : null!,
            OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10240, 0) ? new Avae.Essentials.Avalonia.BrowserImplementation() : null!,
            OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10240, 0) ? new Avae.Essentials.Avalonia.ClipboardImplementation() : null!,
            OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10240, 0) ? new Avae.Essentials.Avalonia.CompassImplementation() : null!,
            OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10240, 0) ? new Avae.Essentials.Avalonia.ConnectivityImplementation() : null!,
            OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10240, 0) ? new Avae.Essentials.Avalonia.ContactsImplementation() : null!,
            OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10240, 0) ? new Avae.Essentials.Avalonia.AvaeDeviceDisplay() : null!,
            OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10240, 0) ? new Avae.Essentials.Avalonia.DeviceInfoImplementation() : null!,
            new Avae.Essentials.Avalonia.AvaeEmail(),
            filepicker,
            fileSystem,
            OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10240, 0) ? new Avae.Essentials.Avalonia.FlashlightImplementation() : null!,
#if WINDOWS
            Geocoding.Default,
#else
            new Avae.Essentials.Avalonia.AvaeGeocoding(),
#endif
            null!,
            OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10240, 0) ? new Avae.Essentials.Avalonia.GyroscopeImplementation() : null!,
            hapticFeedback,
            OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10240, 0) ? new Avae.Essentials.Avalonia.LauncherImplementation() : null!,
            OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10240, 0) ? new Avae.Essentials.Avalonia.MagnetometerImplementation() : null!,
            OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10240, 0) ? new Avae.Essentials.Avalonia.MapImplementation() : null!,
            OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10240, 0) ? new Avae.Essentials.Avalonia.AvaeMediaPicker((AvaloniaMediaPicker)mediapicker) : null!,
            OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10240, 0) ? new Avae.Essentials.Avalonia.OrientationSensorImplementation() : null!,
            new Avae.Essentials.Avalonia.AvaePhoneDialer(),
            preferences,
            screenshot,
            () => OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10240, 0) ? new Avae.Essentials.Avalonia.SecureStorageImplementation() : null!,
            OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10240, 0) ? new Avae.Essentials.Avalonia.AvaeSemanticScreenReader() : null!,
            OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10240, 0) ? new Avae.Essentials.Avalonia.ShareImplementation() : null!,
            OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10240, 0) ? new Avae.Essentials.Avalonia.SmsImplementation() : null!,
            OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10240, 0) ? new Avae.Essentials.Avalonia.TextToSpeechImplementation() : null!,
            OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10240, 0) ? new Avae.Essentials.Avalonia.VibrationImplementation() : null!,
            webAuthenticator);
#elif BROWSER
        services.SetDefaults(
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            filepicker,
            fileSystem,
            null!,
            null!,
            null!,
            null!,
            hapticFeedback,
            null!,
            null!,
            null!,
            mediapicker,
            null!,
            null!,
            preferences,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            webAuthenticator);
#endif

        services.RegisterEssentials();
    }
}
