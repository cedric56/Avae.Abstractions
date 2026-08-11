using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Maui.Essentials;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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
using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;
#if MACOS
//using Microsoft.Maui.Platforms.MacOS.Essentials;
#endif
namespace Avae.Essentials
{
    public static class Extensions
    {
        private static void UseAvaloniaEssentials(this IServiceCollection services,
            IAccelerometer accelerometer,
            IAppActions appActions,
            IAppInfo appInfo,
            IBarometer barometer,
            IBattery battery,
            IBrowser browser,
            Microsoft.Maui.ApplicationModel.DataTransfer.IClipboard clipboard,
            ICompass compass,
            IConnectivity connectivity,
            IContacts contacts,
            IDeviceDisplay deviceDisplay,
            IDeviceInfo deviceInfo,
            IEmail email,
            IFlashlight flashlight,
            IGeocoding geocoding,
            IGeolocation geolocation,
            IGyroscope gyroscope,
            ILauncher launcher,
            IMagnetometer magnetometer,
            IMap map,
            IOrientationSensor orientationSensor,
            IPhoneDialer phoneDialer,
            Func<ISecureStorage> secureStorage,
            ISemanticScreenReader semanticScreenReader,
            IShare share,
            ISms sms,
            ITextToSpeech textToSpeech,
            IVibration vibration)
        {
            var platformProvider = new Avae.Essentials.AvaeTopLevelStateManager();
            EssentialsDefaults.SetScreenshot(null, new Avalonia.Controls.Maui.Essentials.AvaloniaScreenshot(platformProvider));
            EssentialsDefaults.SetFilePicker(null, (IFilePicker)AvaloniaDefaults.CreateAvaloniaFilePicker(platformProvider));
            EssentialsDefaults.SetMediaPicker(null, (IMediaPicker)AvaloniaDefaults.CreateAvaloniaMediaPicker(platformProvider));
            EssentialsDefaults.SetHapticFeedback(null, new Avalonia.Controls.Maui.Essentials.AvaloniaHapticFeedback());
            EssentialsDefaults.SetPreferences(null, new Avalonia.Controls.Maui.Essentials.AvaloniaPreferences());
            EssentialsDefaults.SetFileSystem(null, new Avalonia.Controls.Maui.Essentials.AvaloniaFileSystem());
            EssentialsDefaults.SetWebAuthenticator(null, (IWebAuthenticator)AvaloniaDefaults.CreateAvaloniaWebAuthenticator(platformProvider));



            EssentialsDefaults.SetAccelerometer(null, accelerometer);
            EssentialsDefaults.SetAppActions(null, appActions);
            EssentialsDefaults.SetAppInfo(null, appInfo);
            EssentialsDefaults.SetBarometer(null, barometer);
            EssentialsDefaults.SetBattery(null, battery);
            EssentialsDefaults.SetBrowser(null, browser);
            EssentialsDefaults.SetClipboard(null, clipboard);
            EssentialsDefaults.SetCompass(null, compass);
            EssentialsDefaults.SetConnectivity(null, connectivity);
            EssentialsDefaults.SetContacts(null, contacts);
            EssentialsDefaults.SetDeviceDisplay(null, deviceDisplay);
            EssentialsDefaults.SetDeviceInfo(null, deviceInfo);
            EssentialsDefaults.SetEmail(null, email);
            EssentialsDefaults.SetFlashlight(null, flashlight);
            EssentialsDefaults.SetGeocoding(null, geocoding);
            EssentialsDefaults.SetGeocolation(null, geolocation);
            EssentialsDefaults.SetGyroscope(null, gyroscope);
            EssentialsDefaults.SetLauncher(null, launcher);
            EssentialsDefaults.SetMagnetometer(null, magnetometer);
            EssentialsDefaults.SetMap(null, map);
            EssentialsDefaults.SetOrientationSensor(null, orientationSensor);
            EssentialsDefaults.SetPhoneDialer(null, phoneDialer);
            EssentialsDefaults.SetSecureStorage(null, secureStorage.Invoke());
            EssentialsDefaults.SetSemanticScreenReader(null, semanticScreenReader);
            EssentialsDefaults.SetShare(null, share);
            EssentialsDefaults.SetSms(null, sms);
            EssentialsDefaults.SetTextToSpeech(null, textToSpeech);
            EssentialsDefaults.SetVibration(null, vibration);
        }

        [SupportedOSPlatform("browser")]
        public static async Task UseBrowserEssentials(this IServiceCollection services, string projectName)
        {
            await JSHost.ImportAsync("essentials", $"/_content/{projectName}/essentials.js");
            services.UseAvaloniaEssentials(
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null);

            services.RegisterServices();
        }

        [SupportedOSPlatform("windows")]
        [SupportedOSPlatform("macos")]
        [SupportedOSPlatform("ios")]
        [SupportedOSPlatform("android")]
        public static void UseAvaeEssentials(this IServiceCollection services, string? projectName = null)
        {
#if MACOS
            services.UseAvaloniaEssentials(
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null);

#elif WINDOWS_OS && !IOS && !ANDROID

            services.UseAvaloniaEssentials(
                new Avae.Essentials.AccelerometerImplementation(),
                new Avae.Essentials.AppActionsImplementation(),
                new Avae.Essentials.AppInfoImplementation(),
                new Avae.Essentials.BarometerImplementation(),
                new Avae.Essentials.BatteryImplementation(),
                new Avae.Essentials.BrowserImplementation(),
                new Avae.Essentials.ClipboardImplementation(),
                new Avae.Essentials.CompassImplementation(),
                new Avae.Essentials.ConnectivityImplementation(),
                new Avae.Essentials.ContactsImplementation(),
                new Avae.Essentials.DeviceDisplayImplementation(),
                new Avae.Essentials.DeviceInfoImplementation(),
                new Avae.Essentials.AvaeEmail(),
                new Avae.Essentials.FlashlightImplementation(),
#if WINDOWS
                Geocoding.Default,
#else
                new Avae.Essentials.AvaeGeocoding(),
#endif
                null,
                new Avae.Essentials.GyroscopeImplementation(),
                new Avae.Essentials.LauncherImplementation(),
                new Avae.Essentials.MagnetometerImplementation(),
                new Avae.Essentials.MapImplementation(),
                new Avae.Essentials.OrientationSensorImplementation(),
                new Avae.Essentials.AvaePhoneDialer(),
                () => new Avae.Essentials.SecureStorageImplementation(),
                new Avae.Essentials.AvaeSemanticScreenReader(),
                new Avae.Essentials.ShareImplementation(),
                new Avae.Essentials.SmsImplementation(),
                new Avae.Essentials.TextToSpeechImplementation(),
                new Avae.Essentials.VibrationImplementation());

#endif

            services.RegisterServices();
        }

        private static void RegisterServices(this IServiceCollection services)
        {
            services.TryAddSingleton<IAccelerometer>(Accelerometer.Default);
            services.TryAddSingleton<IAppActions>(AppActions.Current);
            services.TryAddSingleton<IAppInfo>(AppInfo.Current);
            services.TryAddSingleton<IBarometer>(Barometer.Default);
            services.TryAddSingleton<IBattery>(Battery.Default);
            services.TryAddSingleton<IBrowser>(Browser.Default);
            services.TryAddSingleton<IClipboard>(Clipboard.Default);
            services.TryAddSingleton<ICompass>(Compass.Default);
            services.TryAddSingleton<IConnectivity>(Connectivity.Current);
            services.TryAddSingleton<IContacts>(Microsoft.Maui.ApplicationModel.Communication.Contacts.Default);
            services.TryAddSingleton<IDeviceDisplay>(DeviceDisplay.Current);
            services.TryAddSingleton<IDeviceInfo>(DeviceInfo.Current);
            services.TryAddSingleton<IEmail>(Email.Default);
            services.TryAddSingleton<IFilePicker>(FilePicker.Default);
            services.TryAddSingleton<IFlashlight>(Flashlight.Default);
            services.TryAddSingleton<IGeocoding>(Geocoding.Default);
            services.TryAddSingleton<IGeolocation>(Geolocation.Default);
            services.TryAddSingleton<IGyroscope>(Gyroscope.Default);
            services.TryAddSingleton<IHapticFeedback>(HapticFeedback.Default);
            services.TryAddSingleton<ILauncher>(Launcher.Default);
            services.TryAddSingleton<IMagnetometer>(Magnetometer.Default);
            services.TryAddSingleton<IMap>(Map.Default);
            services.TryAddSingleton<IMediaPicker>(MediaPicker.Default);
            services.TryAddSingleton<IOrientationSensor>(OrientationSensor.Default);
            services.TryAddSingleton<IPhoneDialer>(PhoneDialer.Default);
            services.TryAddSingleton<ISecureStorage>(SecureStorage.Default);
            services.TryAddSingleton<ISemanticScreenReader>(SemanticScreenReader.Default);
            services.TryAddSingleton<IShare>(Share.Default);
            services.TryAddSingleton<ISms>(Sms.Default);
            services.TryAddSingleton<ITextToSpeech>(TextToSpeech.Default);
            services.TryAddSingleton<IVibration>(Vibration.Default);
            services.TryAddSingleton<IWebAuthenticator>(WebAuthenticator.Default);
        }
    }
}
