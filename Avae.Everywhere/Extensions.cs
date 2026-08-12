#if BROWSER
using Avalonia;
using Avalonia.Browser;
using System.Runtime.Versioning;
#endif
using Avae.Essentials;
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

namespace Avae.Everywhere
{
    public static class Extensions
    {
#if BROWSER

        [SupportedOSPlatform("browser")]
        public static async Task UseEmbeddedAvaloniaApp(this IServiceCollection services, string appDiv = "app")
        {
            EmbeddedAvalonia.AppDiv = appDiv;
            var builder = AppBuilder.Configure<EmbeddedAvalonia>();
            await builder.SetupBrowserAppAsync();
            services.UseAvaeEssentials();
        }

#endif

        private static void UseAvaloniaEssentials(this IServiceCollection services,
            IAccelerometer accelerometer,
            IAppActions appActions,
            IAppInfo appInfo,
            IBarometer barometer,
            IBattery battery,
            IBrowser browser,
            IClipboard clipboard,
            ICompass compass,
            IConnectivity connectivity,
            IContacts contacts,
            IDeviceDisplay deviceDisplay,
            IDeviceInfo deviceInfo,
            IEmail email,
            IFilePicker filepicker,
            IFileSystem fileSystem,
            IFlashlight flashlight,
            IGeocoding geocoding,
            IGeolocation geolocation,
            IGyroscope gyroscope,
            IHapticFeedback hapticFeedback,
            ILauncher launcher,
            IMagnetometer magnetometer,
            IMap map,
            IMediaPicker mediaPicker,
            IOrientationSensor orientationSensor,
            IPhoneDialer phoneDialer,
            IPreferences preferences,
            IScreenshot screenshot,
            Func<ISecureStorage> secureStorage,
            ISemanticScreenReader semanticScreenReader,
            IShare share,
            ISms sms,
            ITextToSpeech textToSpeech,
            IVibration vibration, 
            IWebAuthenticator webAuthenticator)
        {
            EssentialsDefaults.SetScreenshot(null, screenshot);
            EssentialsDefaults.SetFilePicker(null, filepicker);
            EssentialsDefaults.SetMediaPicker(null, mediaPicker);
            EssentialsDefaults.SetHapticFeedback(null, hapticFeedback);
            EssentialsDefaults.SetPreferences(null, preferences);
            EssentialsDefaults.SetFileSystem(null, fileSystem);
            EssentialsDefaults.SetWebAuthenticator(null, webAuthenticator);            
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
            EssentialsDefaults.SetSecureStorage(null, secureStorage?.Invoke());
            EssentialsDefaults.SetSemanticScreenReader(null, semanticScreenReader);
            EssentialsDefaults.SetShare(null, share);
            EssentialsDefaults.SetSms(null, sms);
            EssentialsDefaults.SetTextToSpeech(null, textToSpeech);
            EssentialsDefaults.SetVibration(null, vibration);
        }

        public static void UseAvaeEssentials(this IServiceCollection services)
        {
            var platformProvider = new AvaeTopLevelStateManager();
            var screenshot = new Avalonia.Controls.Maui.Essentials.AvaloniaScreenshot(platformProvider);
            var filepicker = (IFilePicker)AvaloniaDefaults.CreateAvaloniaFilePicker(platformProvider);
            var mediapicker = (IMediaPicker)AvaloniaDefaults.CreateAvaloniaMediaPicker(platformProvider);
            var hapticFeedback = new Avalonia.Controls.Maui.Essentials.AvaloniaHapticFeedback();
            var preferences = new Avalonia.Controls.Maui.Essentials.AvaloniaPreferences();
            var fileSystem = new Avalonia.Controls.Maui.Essentials.AvaloniaFileSystem();
            var webAuthenticator = (IWebAuthenticator)AvaloniaDefaults.CreateAvaloniaWebAuthenticator(platformProvider);

#if MACOS
            services.UseAvaloniaEssentials(
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

                services.UseAvaloniaEssentials(
                    new Avae.Everywhere.AccelerometerImplementation(),
                    new Avae.Everywhere.AppActionsImplementation(),
                    new Avae.Everywhere.AppInfoImplementation(),
                    new Avae.Everywhere.BarometerImplementation(),
                    new Avae.Everywhere.BatteryImplementation(),
                    new Avae.Everywhere.BrowserImplementation(),
                    new Avae.Everywhere.ClipboardImplementation(),
                    new Avae.Everywhere.CompassImplementation(),
                    new Avae.Everywhere.ConnectivityImplementation(),
                    new Avae.Everywhere.ContactsImplementation(),
                    new Avae.Everywhere.DeviceDisplayImplementation(),
                    new Avae.Everywhere.DeviceInfoImplementation(),
                    new Avae.Everywhere.AvaeEmail(),
                    filepicker,
                    fileSystem,
                    new Avae.Everywhere.FlashlightImplementation(),
#if WINDOWS
                Geocoding.Default,
#else
                    new Avae.Everywhere.AvaeGeocoding(),
#endif
                    null,
                    new Avae.Everywhere.GyroscopeImplementation(),
                    hapticFeedback,
                    new Avae.Everywhere.LauncherImplementation(),
                    new Avae.Everywhere.MagnetometerImplementation(),
                    new Avae.Everywhere.MapImplementation(),
                    mediapicker,
                    new Avae.Everywhere.OrientationSensorImplementation(),
                    new Avae.Everywhere.AvaePhoneDialer(),
                    preferences,
                    screenshot,
                    () => new Avae.Everywhere.SecureStorageImplementation(),
                    new Avae.Everywhere.AvaeSemanticScreenReader(),
                    new Avae.Everywhere.ShareImplementation(),
                    new Avae.Everywhere.SmsImplementation(),
                    new Avae.Everywhere.TextToSpeechImplementation(),
                    new Avae.Everywhere.VibrationImplementation(),
                    webAuthenticator);
#elif BROWSER
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
                filepicker,
                fileSystem,
                null,
                null,
                null,
                null,
                hapticFeedback,
                null,
                null,
                null,
                mediapicker,
                null,
                null,
                preferences,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                webAuthenticator);
#endif
            services.RegisterEssentials();
        }
    }
}
