using Avalonia.Controls.Maui.Essentials;
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
using System.Runtime.CompilerServices;

namespace Avae.Everywhere
{
    static class LinuxDefaults
    {
        [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
        [return: UnsafeAccessorType("Microsoft.Maui.Platforms.MacOS.Essentials.AvaloniaFilePicker, Microsoft.Maui.Platforms.MacOS.Essentials")]
        internal extern static object CreateAccelerometer();

        [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
        [return: UnsafeAccessorType("Microsoft.Maui.Platforms.MacOS.Essentials.AvaloniaMediaPicker, Microsoft.Maui.Platforms.MacOS.Essentials")]
        internal extern static object CreateAppActions();

        [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
        [return: UnsafeAccessorType("Microsoft.Maui.Platforms.MacOS.Essentials.AvaloniaWebAuthenticator, Microsoft.Maui.Platforms.MacOS.Essentials")]
        internal extern static object CreateAppInfo();

        [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
        [return: UnsafeAccessorType("Microsoft.Maui.Platforms.MacOS.Essentials.AvaloniaFilePicker, Microsoft.Maui.Platforms.MacOS.Essentials")]
        internal extern static object CreateBarometer();

        [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
        [return: UnsafeAccessorType("Microsoft.Maui.Platforms.MacOS.Essentials.AvaloniaMediaPicker, Microsoft.Maui.Platforms.MacOS.Essentials")]
        internal extern static object CreateBattery();

        [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
        [return: UnsafeAccessorType("Microsoft.Maui.Platforms.MacOS.Essentials.AvaloniaWebAuthenticator, Microsoft.Maui.Platforms.MacOS.Essentials")]
        internal extern static object CreateBrowser();

        [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
        [return: UnsafeAccessorType("Microsoft.Maui.Platforms.MacOS.Essentials.AvaloniaFilePicker, Microsoft.Maui.Platforms.MacOS.Essentials")]
        internal extern static object CreateClipboard();

        [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
        [return: UnsafeAccessorType("Microsoft.Maui.Platforms.MacOS.Essentials.AvaloniaMediaPicker, Microsoft.Maui.Platforms.MacOS.Essentials")]
        internal extern static object CreateCompass();

        [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
        [return: UnsafeAccessorType("Microsoft.Maui.Platforms.MacOS.Essentials.AvaloniaWebAuthenticator, Microsoft.Maui.Platforms.MacOS.Essentials")]
        internal extern static object CreateConnectivity();

        [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
        [return: UnsafeAccessorType("Microsoft.Maui.Platforms.MacOS.Essentials.AvaloniaFilePicker, Microsoft.Maui.Platforms.MacOS.Essentials")]
        internal extern static object CreateContacts();

        [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
        [return: UnsafeAccessorType("Microsoft.Maui.Platforms.MacOS.Essentials.AvaloniaMediaPicker, Microsoft.Maui.Platforms.MacOS.Essentials")]
        internal extern static object CreateDeviceDisplay();

        [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
        [return: UnsafeAccessorType("Microsoft.Maui.Platforms.MacOS.Essentials.AvaloniaWebAuthenticator, Microsoft.Maui.Platforms.MacOS.Essentials")]
        internal extern static object CreateDeviceInfo();

        [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
        [return: UnsafeAccessorType("Microsoft.Maui.Platforms.MacOS.Essentials.AvaloniaFilePicker, Microsoft.Maui.Platforms.MacOS.Essentials")]
        internal extern static object CreateEmail();

        [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
        [return: UnsafeAccessorType("Microsoft.Maui.Platforms.MacOS.Essentials.AvaloniaMediaPicker, Microsoft.Maui.Platforms.MacOS.Essentials")]
        internal extern static object CreateFlashlight();

        [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
        [return: UnsafeAccessorType("Microsoft.Maui.Platforms.MacOS.Essentials.AvaloniaWebAuthenticator, Microsoft.Maui.Platforms.MacOS.Essentials")]
        internal extern static object CreateGeocoding();

        [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
        [return: UnsafeAccessorType("Microsoft.Maui.Platforms.MacOS.Essentials.AvaloniaFilePicker, AMicrosoft.Maui.Platforms.MacOS.Essentials")]
        internal extern static object CreateGeolocation();

        [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
        [return: UnsafeAccessorType("Microsoft.Maui.Platforms.MacOS.Essentials.AvaloniaMediaPicker, Microsoft.Maui.Platforms.MacOS.Essentials")]
        internal extern static object CreateGyroscope();

        [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
        [return: UnsafeAccessorType("Microsoft.Maui.Platforms.MacOS.Essentials.AvaloniaWebAuthenticator, Microsoft.Maui.Platforms.MacOS.Essentials")]
        internal extern static object CreateHapticFeedback();

        [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
        [return: UnsafeAccessorType("Microsoft.Maui.Platforms.MacOS.Essentials.AvaloniaFilePicker, Microsoft.Maui.Platforms.MacOS.Essentials")]
        internal extern static object CreateLauncher();

        [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
        [return: UnsafeAccessorType("Microsoft.Maui.Platforms.MacOS.Essentials.AvaloniaMediaPicker, Microsoft.Maui.Platforms.MacOS.Essentials")]
        internal extern static object CreateMagnetometer();

        [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
        [return: UnsafeAccessorType("Microsoft.Maui.Platforms.MacOS.Essentials.AvaloniaWebAuthenticator, Microsoft.Maui.Platforms.MacOS.Essentials")]
        internal extern static object CreateMap();

        [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
        [return: UnsafeAccessorType("Microsoft.Maui.Platforms.MacOS.Essentials.AvaloniaFilePicker, Microsoft.Maui.Platforms.MacOS.Essentials")]
        internal extern static object CreateOrientationSensor();

        [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
        [return: UnsafeAccessorType("Microsoft.Maui.Platforms.MacOS.Essentials.AvaloniaMediaPicker, Microsoft.Maui.Platforms.MacOS.Essentials")]
        internal extern static object CreatePhoneDialer();

        [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
        [return: UnsafeAccessorType("Microsoft.Maui.Platforms.MacOS.Essentials.AvaloniaWebAuthenticator, Microsoft.Maui.Platforms.MacOS.Essentials")]
        internal extern static object CreateSecureStorage();

        [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
        [return: UnsafeAccessorType("Microsoft.Maui.Platforms.MacOS.Essentials.AvaloniaWebAuthenticator, Microsoft.Maui.Platforms.MacOS.Essentials")]
        internal extern static object CreateSemanticScreenReader();

        [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
        [return: UnsafeAccessorType("Microsoft.Maui.Platforms.MacOS.Essentials.AvaloniaWebAuthenticator, Microsoft.Maui.Platforms.MacOS.Essentials")]
        internal extern static object CreateShare();

        [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
        [return: UnsafeAccessorType("Microsoft.Maui.Platforms.MacOS.Essentials.AvaloniaWebAuthenticator, Microsoft.Maui.Platforms.MacOS.Essentials")]
        internal extern static object CreateSms();

        [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
        [return: UnsafeAccessorType("Microsoft.Maui.Platforms.MacOS.Essentials.AvaloniaWebAuthenticator, Microsoft.Maui.Platforms.MacOS.Essentials")]
        internal extern static object CreateTextToSpeech();

        [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
        [return: UnsafeAccessorType("Microsoft.Maui.Platforms.MacOS.Essentials.AvaloniaWebAuthenticator, Microsoft.Maui.Platforms.MacOS.Essentials")]
        internal extern static object CreateVibratioh();

        [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
        [return: UnsafeAccessorType("Microsoft.Maui.Platforms.MacOS.Essentials.AvaloniaFilePicker, Microsoft.Maui.Platforms.MacOS.Essentials")]
        internal extern static object CreateFilePicker();

        [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
        [return: UnsafeAccessorType("Microsoft.Maui.Platforms.MacOS.Essentials.AvaloniaMediaPicker, Microsoft.Maui.Platforms.MacOS.Essentials")]
        internal extern static object CreateMediaPicker();

        [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
        [return: UnsafeAccessorType("Microsoft.Maui.Platforms.MacOS.Essentials.AvaloniaWebAuthenticator, Microsoft.Maui.Platforms.MacOS.Essentials")]
        internal extern static object CreateWebAuthenticator();

        [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
        [return: UnsafeAccessorType("Microsoft.Maui.Platforms.MacOS.Essentials.AvaloniaWebAuthenticator, Microsoft.Maui.Platforms.MacOS.Essentials")]
        internal extern static object CreateScreenshot();

        [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
        [return: UnsafeAccessorType("Microsoft.Maui.Platforms.MacOS.Essentials.AvaloniaWebAuthenticator, Microsoft.Maui.Platforms.MacOS.Essentials")]
        internal extern static object CreatePreferences();

        [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
        [return: UnsafeAccessorType("Microsoft.Maui.Platforms.MacOS.Essentials.AvaloniaWebAuthenticator, Microsoft.Maui.Platforms.MacOS.Essentials")]
        internal extern static object CreateFileSystem();
    }

    static class MacosDefaults
    {
        //[UnsafeAccessor(UnsafeAccessorKind.Constructor)]
        //[return: UnsafeAccessorType("Microsoft.Maui.Platforms.MacOS.Essentials.AvaloniaFilePicker, Microsoft.Maui.Platforms.MacOS.Essentials")]
        //internal extern static object CreateAccelerometer();

        //[UnsafeAccessor(UnsafeAccessorKind.Constructor)]
        //[return: UnsafeAccessorType("Microsoft.Maui.Platforms.MacOS.Essentials.AvaloniaMediaPicker, Microsoft.Maui.Platforms.MacOS.Essentials")]
        //internal extern static object CreateAppActions();

        [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
        [return: UnsafeAccessorType("Microsoft.Maui.Platforms.MacOS.Essentials.AppInfoImplementation, Microsoft.Maui.Platforms.MacOS.Essentials")]
        internal extern static object CreateAppInfo();

        //[UnsafeAccessor(UnsafeAccessorKind.Constructor)]
        //[return: UnsafeAccessorType("Microsoft.Maui.Platforms.MacOS.Essentials.AvaloniaFilePicker, Microsoft.Maui.Platforms.MacOS.Essentials")]
        //internal extern static object CreateBarometer();

        [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
        [return: UnsafeAccessorType("Microsoft.Maui.Platforms.MacOS.Essentials.BatteryImplementation, Microsoft.Maui.Platforms.MacOS.Essentials")]
        internal extern static object CreateBattery();

        [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
        [return: UnsafeAccessorType("Microsoft.Maui.Platforms.MacOS.Essentials.BrowserImplementation, Microsoft.Maui.Platforms.MacOS.Essentials")]
        internal extern static object CreateBrowser();

        [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
        [return: UnsafeAccessorType("Microsoft.Maui.Platforms.MacOS.Essentials.ClipboardImplementation, Microsoft.Maui.Platforms.MacOS.Essentials")]
        internal extern static object CreateClipboard();

        //[UnsafeAccessor(UnsafeAccessorKind.Constructor)]
        //[return: UnsafeAccessorType("Microsoft.Maui.Platforms.MacOS.Essentials.AvaloniaMediaPicker, Microsoft.Maui.Platforms.MacOS.Essentials")]
        //internal extern static object CreateCompass();

        [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
        [return: UnsafeAccessorType("Microsoft.Maui.Platforms.MacOS.Essentials.ConnectivtyImplementation, Microsoft.Maui.Platforms.MacOS.Essentials")]
        internal extern static object CreateConnectivity();

        //[UnsafeAccessor(UnsafeAccessorKind.Constructor)]
        //[return: UnsafeAccessorType("Microsoft.Maui.Platforms.MacOS.Essentials.AvaloniaFilePicker, Microsoft.Maui.Platforms.MacOS.Essentials")]
        //internal extern static object CreateContacts();

        [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
        [return: UnsafeAccessorType("Microsoft.Maui.Platforms.MacOS.Essentials.DeviceDisplayImplementation, Microsoft.Maui.Platforms.MacOS.Essentials")]
        internal extern static object CreateDeviceDisplay();

        [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
        [return: UnsafeAccessorType("Microsoft.Maui.Platforms.MacOS.Essentials.DeviceInfoImplementation, Microsoft.Maui.Platforms.MacOS.Essentials")]
        internal extern static object CreateDeviceInfo();

        [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
        [return: UnsafeAccessorType("Microsoft.Maui.Platforms.MacOS.Essentials.EmailImplementation, Microsoft.Maui.Platforms.MacOS.Essentials")]
        internal extern static object CreateEmail();

        [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
        [return: UnsafeAccessorType("Microsoft.Maui.Platforms.MacOS.Essentials.FlashlightImplementation, Microsoft.Maui.Platforms.MacOS.Essentials")]
        internal extern static object CreateFlashlight();

        //[UnsafeAccessor(UnsafeAccessorKind.Constructor)]
        //[return: UnsafeAccessorType("Microsoft.Maui.Platforms.MacOS.Essentials.AvaloniaWebAuthenticator, Microsoft.Maui.Platforms.MacOS.Essentials")]
        //internal extern static object CreateGeocoding();

        [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
        [return: UnsafeAccessorType("Microsoft.Maui.Platforms.MacOS.Essentials.GeolocationImplementation, AMicrosoft.Maui.Platforms.MacOS.Essentials")]
        internal extern static object CreateGeolocation();

        //[UnsafeAccessor(UnsafeAccessorKind.Constructor)]
        //[return: UnsafeAccessorType("Microsoft.Maui.Platforms.MacOS.Essentials.AvaloniaMediaPicker, Microsoft.Maui.Platforms.MacOS.Essentials")]
        //internal extern static object CreateGyroscope();

        [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
        [return: UnsafeAccessorType("Microsoft.Maui.Platforms.MacOS.Essentials.HapticFeedBackImplementation, Microsoft.Maui.Platforms.MacOS.Essentials")]
        internal extern static object CreateHapticFeedback();

        [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
        [return: UnsafeAccessorType("Microsoft.Maui.Platforms.MacOS.Essentials.LauncherImplementation, Microsoft.Maui.Platforms.MacOS.Essentials")]
        internal extern static object CreateLauncher();

        //[UnsafeAccessor(UnsafeAccessorKind.Constructor)]
        //[return: UnsafeAccessorType("Microsoft.Maui.Platforms.MacOS.Essentials.AvaloniaMediaPicker, Microsoft.Maui.Platforms.MacOS.Essentials")]
        //internal extern static object CreateMagnetometer();

        [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
        [return: UnsafeAccessorType("Microsoft.Maui.Platforms.MacOS.Essentials.MapImplementation, Microsoft.Maui.Platforms.MacOS.Essentials")]
        internal extern static object CreateMap();

        //[UnsafeAccessor(UnsafeAccessorKind.Constructor)]
        //[return: UnsafeAccessorType("Microsoft.Maui.Platforms.MacOS.Essentials.AvaloniaFilePicker, Microsoft.Maui.Platforms.MacOS.Essentials")]
        //internal extern static object CreateOrientationSensor();

        [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
        [return: UnsafeAccessorType("Microsoft.Maui.Platforms.MacOS.Essentials.PhoneDialerImplementation, Microsoft.Maui.Platforms.MacOS.Essentials")]
        internal extern static object CreatePhoneDialer();

        [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
        [return: UnsafeAccessorType("Microsoft.Maui.Platforms.MacOS.Essentials.SecureStorageImplementation, Microsoft.Maui.Platforms.MacOS.Essentials")]
        internal extern static object CreateSecureStorage();

        [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
        [return: UnsafeAccessorType("Microsoft.Maui.Platforms.MacOS.Essentials.SemanticScreenReaderImplementation, Microsoft.Maui.Platforms.MacOS.Essentials")]
        internal extern static object CreateSemanticScreenReader();

        [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
        [return: UnsafeAccessorType("Microsoft.Maui.Platforms.MacOS.Essentials.ShareImplementation, Microsoft.Maui.Platforms.MacOS.Essentials")]
        internal extern static object CreateShare();

        [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
        [return: UnsafeAccessorType("Microsoft.Maui.Platforms.MacOS.Essentials.SmsImplementation, Microsoft.Maui.Platforms.MacOS.Essentials")]
        internal extern static object CreateSms();

        [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
        [return: UnsafeAccessorType("Microsoft.Maui.Platforms.MacOS.Essentials.TextToSpeechImplementation, Microsoft.Maui.Platforms.MacOS.Essentials")]
        internal extern static object CreateTextToSpeech();

        [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
        [return: UnsafeAccessorType("Microsoft.Maui.Platforms.MacOS.Essentials.VibrationImplementation, Microsoft.Maui.Platforms.MacOS.Essentials")]
        internal extern static object CreateVibration();

        [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
        [return: UnsafeAccessorType("Microsoft.Maui.Platforms.MacOS.Essentials.FilePickerImplementation, Microsoft.Maui.Platforms.MacOS.Essentials")]
        internal extern static object CreateFilePicker();

        [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
        [return: UnsafeAccessorType("Microsoft.Maui.Platforms.MacOS.Essentials.MediaPickerImplementation, Microsoft.Maui.Platforms.MacOS.Essentials")]
        internal extern static object CreateMediaPicker();

        //[UnsafeAccessor(UnsafeAccessorKind.Constructor)]
        //[return: UnsafeAccessorType("Microsoft.Maui.Platforms.MacOS.Essentials.AvaloniaWebAuthenticator, Microsoft.Maui.Platforms.MacOS.Essentials")]
        //internal extern static object CreateWebAuthenticator();

        [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
        [return: UnsafeAccessorType("Microsoft.Maui.Platforms.MacOS.Essentials.ScreenshotImplementation, Microsoft.Maui.Platforms.MacOS.Essentials")]
        internal extern static object CreateScreenshot();

        [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
        [return: UnsafeAccessorType("Microsoft.Maui.Platforms.MacOS.Essentials.PreferencesImplementation, Microsoft.Maui.Platforms.MacOS.Essentials")]
        internal extern static object CreatePreferences();

        [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
        [return: UnsafeAccessorType("Microsoft.Maui.Platforms.MacOS.Essentials.FileSystemImplementation, Microsoft.Maui.Platforms.MacOS.Essentials")]
        internal extern static object CreateFileSystem();
    }

    public static class AvaloniaDefaults
    {
        [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
        [return: UnsafeAccessorType("Avalonia.Controls.Maui.Essentials.AvaloniaFilePicker, Avalonia.Controls.Maui.Essentials")]
        public extern static object CreateAvaloniaFilePicker(IAvaloniaEssentialsPlatformProvider provider);

        [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
        [return: UnsafeAccessorType("Avalonia.Controls.Maui.Essentials.AvaloniaMediaPicker, Avalonia.Controls.Maui.Essentials")]
        internal extern static object CreateAvaloniaMediaPicker(IAvaloniaEssentialsPlatformProvider provider);

        [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
        [return: UnsafeAccessorType("Avalonia.Controls.Maui.Essentials.AvaloniaWebAuthenticator, Avalonia.Controls.Maui.Essentials")]
        internal extern static object CreateAvaloniaWebAuthenticator(IAvaloniaEssentialsPlatformProvider provider);
    }

    /// <summary>
    /// Installs Avalonia implementations into the Microsoft.Maui.Essentials static facades.
    /// The facades only expose internal SetDefault/SetCurrent hooks, so these accessors use
    /// <see cref="UnsafeAccessorAttribute"/> instead of MAUI internals.
    /// </summary>
    static class EssentialsDefaults
    {
        [UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = "SetDefault")]
        internal static extern void SetAccelerometer(
        [UnsafeAccessorType("Microsoft.Maui.Devices.Sensors.Accelerometer, Microsoft.Maui.Essentials")] object? facade,
        IAccelerometer? implementation);

        [UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = "SetCurrent")]
        internal static extern void SetAppActions(
        [UnsafeAccessorType("Microsoft.Maui.ApplicationModel.AppActions, Microsoft.Maui.Essentials")] object? facade,
        IAppActions? implementation);

        [UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = "SetCurrent")]
        internal static extern void SetAppInfo(
        [UnsafeAccessorType("Microsoft.Maui.ApplicationModel.AppInfo, Microsoft.Maui.Essentials")] object? facade,
        IAppInfo? implementation);

        [UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = "SetDefault")]
        internal static extern void SetBarometer(
        [UnsafeAccessorType("Microsoft.Maui.Devices.Sensors.Barometer, Microsoft.Maui.Essentials")] object? facade,
        IBarometer? implementation);

        [UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = "SetDefault")]
        internal static extern void SetBattery(
        [UnsafeAccessorType("Microsoft.Maui.Devices.Battery, Microsoft.Maui.Essentials")] object? facade,
        IBattery? implementation);

        [UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = "SetDefault")]
        internal static extern void SetBrowser(
        [UnsafeAccessorType("Microsoft.Maui.ApplicationModel.Browser, Microsoft.Maui.Essentials")] object? facade,
        IBrowser? implementation);

        [UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = "SetDefault")]
        internal static extern void SetClipboard(
        [UnsafeAccessorType("Microsoft.Maui.ApplicationModel.DataTransfer.Clipboard, Microsoft.Maui.Essentials")] object? facade,
        IClipboard? implementation);

        [UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = "SetDefault")]
        internal static extern void SetCompass(
        [UnsafeAccessorType("Microsoft.Maui.Devices.Sensors.Compass, Microsoft.Maui.Essentials")] object? facade,
        ICompass? implementation);

        [UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = "SetCurrent")]
        internal static extern void SetConnectivity(
        [UnsafeAccessorType("Microsoft.Maui.Networking.Connectivity, Microsoft.Maui.Essentials")] object? facade,
        IConnectivity? implementation);

        [UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = "SetDefault")]
        internal static extern void SetContacts(
        [UnsafeAccessorType("Microsoft.Maui.ApplicationModel.Communication.Contacts, Microsoft.Maui.Essentials")] object? facade,
        IContacts? implementation);

        [UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = "SetCurrent")]
        internal static extern void SetDeviceDisplay(
        [UnsafeAccessorType("Microsoft.Maui.Devices.DeviceDisplay, Microsoft.Maui.Essentials")] object? facade,
        IDeviceDisplay? implementation);

        [UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = "SetCurrent")]
        internal static extern void SetDeviceInfo(
        [UnsafeAccessorType("Microsoft.Maui.Devices.DeviceInfo, Microsoft.Maui.Essentials")] object? facade,
        IDeviceInfo? implementation);

        [UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = "SetDefault")]
        internal static extern void SetEmail(
        [UnsafeAccessorType("Microsoft.Maui.ApplicationModel.Communication.Email, Microsoft.Maui.Essentials")] object? facade,
        IEmail? implementation);

        [UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = "SetDefault")]
        internal static extern void SetFlashlight(
        [UnsafeAccessorType("Microsoft.Maui.Devices.Flashlight, Microsoft.Maui.Essentials")] object? facade,
        IFlashlight? implementation);

        [UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = "SetCurrent")]
        internal static extern void SetGeocoding(
            [UnsafeAccessorType("Microsoft.Maui.Devices.Sensors.Geocoding, Microsoft.Maui.Essentials")] object? facade,
            IGeocoding? implementation);

        [UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = "SetDefault")]
        internal static extern void SetGeocolation(
            [UnsafeAccessorType("Microsoft.Maui.Devices.Sensors.Geolocation, Microsoft.Maui.Essentials")] object? facade,
            IGeolocation? implementation);

        [UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = "SetDefault")]
        internal static extern void SetGyroscope(
        [UnsafeAccessorType("Microsoft.Maui.Devices.Sensors.Gyroscope, Microsoft.Maui.Essentials")] object? facade,
        IGyroscope? implementation);

        [UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = "SetDefault")]
        internal static extern void SetLauncher(
        [UnsafeAccessorType("Microsoft.Maui.ApplicationModel.Launcher, Microsoft.Maui.Essentials")] object? facade,
        ILauncher? implementation);

        [UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = "SetDefault")]
        internal static extern void SetMagnetometer(
        [UnsafeAccessorType("Microsoft.Maui.Devices.Sensors.Magnetometer, Microsoft.Maui.Essentials")] object? facade,
        IMagnetometer? implementation);

        [UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = "SetDefault")]
        internal static extern void SetMap(
        [UnsafeAccessorType("Microsoft.Maui.ApplicationModel.Map, Microsoft.Maui.Essentials")] object? facade,
        IMap? implementation);

        [UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = "SetDefault")]
        internal static extern void SetOrientationSensor(
            [UnsafeAccessorType("Microsoft.Maui.Devices.Sensors.OrientationSensor, Microsoft.Maui.Essentials")] object? facade,
            IOrientationSensor? implementation);
        //null,

        [UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = "SetDefault")]
        internal static extern void SetPhoneDialer(
        [UnsafeAccessorType("Microsoft.Maui.ApplicationModel.Communication.PhoneDialer, Microsoft.Maui.Essentials")] object? facade,
        IPhoneDialer? implementation);

        [UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = "SetDefault")]
        internal static extern void SetSecureStorage(
        [UnsafeAccessorType("Microsoft.Maui.Storage.SecureStorage, Microsoft.Maui.Essentials")] object? facade,
        ISecureStorage? implementation);

        [UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = "SetDefault")]
        internal static extern void SetSemanticScreenReader(
        [UnsafeAccessorType("Microsoft.Maui.Accessibility.SemanticScreenReader, Microsoft.Maui.Essentials")] object? facade,
        ISemanticScreenReader? implementation);

        [UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = "SetDefault")]
        internal static extern void SetShare(
        [UnsafeAccessorType("Microsoft.Maui.ApplicationModel.DataTransfer.Share, Microsoft.Maui.Essentials")] object? facade,
        IShare? implementation);

        [UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = "SetDefault")]
        internal static extern void SetSms(
        [UnsafeAccessorType("Microsoft.Maui.ApplicationModel.Communication.Sms, Microsoft.Maui.Essentials")] object? facade,
        ISms? implementation);

        [UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = "SetDefault")]
        internal static extern void SetTextToSpeech(
        [UnsafeAccessorType("Microsoft.Maui.Media.TextToSpeech, Microsoft.Maui.Essentials")] object? facade,
        ITextToSpeech? implementation);

        [UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = "SetDefault")]
        internal static extern void SetVibration(
        [UnsafeAccessorType("Microsoft.Maui.Devices.Vibration, Microsoft.Maui.Essentials")] object? facade,
        IVibration? implementation);




        [UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = "SetDefault")]
        internal static extern void SetFilePicker(
            [UnsafeAccessorType("Microsoft.Maui.Storage.FilePicker, Microsoft.Maui.Essentials")] object? facade,
            IFilePicker? implementation);

        [UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = "SetCurrent")]
        internal static extern void SetFileSystem(
            [UnsafeAccessorType("Microsoft.Maui.Storage.FileSystem, Microsoft.Maui.Essentials")] object? facade,
            IFileSystem? implementation);

        [UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = "SetDefault")]
        internal static extern void SetHapticFeedback(
            [UnsafeAccessorType("Microsoft.Maui.Devices.HapticFeedback, Microsoft.Maui.Essentials")] object? facade,
            IHapticFeedback? implementation);

        [UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = "SetDefault")]
        internal static extern void SetMediaPicker(
            [UnsafeAccessorType("Microsoft.Maui.Media.MediaPicker, Microsoft.Maui.Essentials")] object? facade,
            IMediaPicker? implementation);

        [UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = "SetDefault")]
        internal static extern void SetPreferences(
            [UnsafeAccessorType("Microsoft.Maui.Storage.Preferences, Microsoft.Maui.Essentials")] object? facade,
            IPreferences? implementation);

        [UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = "SetDefault")]
        internal static extern void SetScreenshot(
            [UnsafeAccessorType("Microsoft.Maui.Media.Screenshot, Microsoft.Maui.Essentials")] object? facade,
            IScreenshot? implementation);

        [UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = "SetDefault")]
        internal static extern void SetWebAuthenticator(
            [UnsafeAccessorType("Microsoft.Maui.Authentication.WebAuthenticator, Microsoft.Maui.Essentials")] object? facade,
            IWebAuthenticator? implementation);
    }
}
