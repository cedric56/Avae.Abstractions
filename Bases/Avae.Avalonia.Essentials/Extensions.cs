using Avae.Essentials;
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

namespace Avae.Avalonia.Essentials;

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
            webAuthenticator,
            VersionTracking.Default);

#elif WINDOWS_OS && !IOS && !ANDROID && !BROWSER

        services.SetDefaults(
            OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10240, 0) ? new AccelerometerImplementation() : null!,
            OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10586, 0) ? new AppActionsImplementation() : null!,
            OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10240, 0) ? new AvaeAppInfo() : null!,
            OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10240, 0) ? new BarometerImplementation() : null!,
            OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10240, 0) ? new BatteryImplementation() : null!,
            OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10240, 0) ? new BrowserImplementation() : null!,
            OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10240, 0) ? new ClipboardImplementation() : null!,
            OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10240, 0) ? new CompassImplementation() : null!,
            OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10240, 0) ? new ConnectivityImplementation() : null!,
            OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10240, 0) ? new ContactsImplementation() : null!,
            OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10240, 0) ? new AvaeDeviceDisplay() : null!,
            OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10240, 0) ? new DeviceInfoImplementation() : null!,
            new Avae.Avalonia.Essentials.AvaeEmail(),
            filepicker,
            fileSystem,
            OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10240, 0) ? new FlashlightImplementation() : null!,
#if WINDOWS
            OperatingSystem.IsWindowsVersionAtLeast(10, 0, 17763, 0) ? Geocoding.Default : null!,
#else
            new Avae.Avalonia.Essentials.AvaeGeocoding(),
#endif
            null!,
            OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10240, 0) ? new GyroscopeImplementation() : null!,
            hapticFeedback,
            OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10240, 0) ? new LauncherImplementation() : null!,
            OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10240, 0) ? new MagnetometerImplementation() : null!,
            OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10240, 0) ? new MapImplementation() : null!,
            OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10240, 0) ? new AvaeMediaPicker((AvaloniaMediaPicker)mediapicker) : null!,
            OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10240, 0) ? new OrientationSensorImplementation() : null!,
            new Avae.Avalonia.Essentials.AvaePhoneDialer(),
            preferences,
            screenshot,
            () => OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10240, 0) ? new SecureStorageImplementation() : null!,
            OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10240, 0) ? new AvaeSemanticScreenReader() : null!,
            OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10240, 0) ? new ShareImplementation() : null!,
            OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10240, 0) ? new SmsImplementation() : null!,
            OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10240, 0) ? new TextToSpeechImplementation() : null!,
            OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10240, 0) ? new VibrationImplementation() : null!,
            webAuthenticator,
            VersionTracking.Default);
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
            webAuthenticator,
            VersionTracking.Default);
#endif

        services.RegisterEssentials();
    }
}
