using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
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
using System.Runtime.InteropServices.JavaScript;
#if MACOS
//using Microsoft.Maui.Platforms.MacOS.Essentials;
#endif
namespace Avae.Essentials
{
    public static class Extensions
    {
        class AvaeTopLevelStateManager : IAvaloniaEssentialsPlatformProvider
        {
            public AvaeTopLevelStateManager()
            {
                TopLevel.GotFocusEvent.AddClassHandler(typeof(TopLevel), (sender, args) =>
                {
                    OnActivated((TopLevel)sender!);
                });
            }

            TopLevel? _active;

            public void OnActivated(TopLevel topLevel)
            {
                if (_active == topLevel)
                    return;

                _active = topLevel;
            }

            public TopLevel? GetTopLevel()
            {
                var lifetime = Avalonia.Application.Current?.ApplicationLifetime;

                var active = lifetime switch
                {
                    IClassicDesktopStyleApplicationLifetime desktop => _active ?? TopLevel.GetTopLevel(desktop.MainWindow),
                    ISingleViewApplicationLifetime singleView => _active ?? TopLevel.GetTopLevel(singleView.MainView),
                    _ => _active ?? TopLevel.GetTopLevel(null)
                };

                return active ?? TopLevel.GetTopLevel(null);
            }
        }
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
            var platformProvider = new AvaeTopLevelStateManager();
            EssentialsDefaults.SetScreenshot(null, new Avalonia.Controls.Maui.Essentials.AvaloniaScreenshot(platformProvider));
            EssentialsDefaults.SetFilePicker(null, (IFilePicker)EssentialsDefaults.CreateAvaloniaFilePicker(platformProvider));
            EssentialsDefaults.SetMediaPicker(null, (IMediaPicker)EssentialsDefaults.CreateAvaloniaMediaPicker(platformProvider));
            EssentialsDefaults.SetHapticFeedback(null, new Avalonia.Controls.Maui.Essentials.AvaloniaHapticFeedback());
            EssentialsDefaults.SetPreferences(null, new Avalonia.Controls.Maui.Essentials.AvaloniaPreferences());
            EssentialsDefaults.SetFileSystem(null, new Avalonia.Controls.Maui.Essentials.AvaloniaFileSystem());
            EssentialsDefaults.SetWebAuthenticator(null, (IWebAuthenticator)EssentialsDefaults.CreateAvaloniaWebAuthenticator(platformProvider));



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

        public static void UseAvaeEssentials(this IServiceCollection services, string? projectName = null)
        {
            if (OperatingSystem.IsBrowser())
                Task.Run(async () => await JSHost.ImportAsync("essentials", $"/_content/{projectName}/essentials.js"));
#if MACOS
            throw new NotImplementedException("TODO");

#elif ANDROID || IOS
            services.UseAvaloniaEssentials(
                Microsoft.Maui.Devices.Sensors.Accelerometer.Default,
                AppActions.Current,
                AppInfo.Current,
                Microsoft.Maui.Devices.Sensors.Barometer.Default,
                Battery.Default,
                Browser.Default,
                Clipboard.Default,
                Microsoft.Maui.Devices.Sensors.Compass.Default,
                Connectivity.Current,
                Microsoft.Maui.ApplicationModel.Communication.Contacts.Default,
                DeviceDisplay.Current,
                DeviceInfo.Current,
                Email.Default,
                Flashlight.Default,
                Geocoding.Default,
                Geolocation.Default,
                Gyroscope.Default,
                Launcher.Default,
                Microsoft.Maui.Devices.Sensors.Magnetometer.Default,
                Map.Default,
                Microsoft.Maui.Devices.Sensors.OrientationSensor.Default,
                PhoneDialer.Default,
                () => SecureStorage.Default,
                SemanticScreenReader.Default,
                Share.Default,
                Sms.Default,
                TextToSpeech.Default,
                Vibration.Default);
#elif IsWindowsOS || IsWebProject
            services.UseAvaloniaEssentials(
                new AccelerometerImplementation(),
                new AppActionsImplementation(),
                new AppInfoImplementation(),
                new BarometerImplementation(),
                new BatteryImplementation(),
                new BrowserImplementation(),
                new ClipboardImplementation(),
                new CompassImplementation(),
                new ConnectivityImplementation(),
                new ContactsImplementation(),
                new DeviceDisplayImplementation(),
                new DeviceInfoImplementation(),
                new EmailImplementation(),
                new FlashlightImplementation(),
                new GeocodingImplementation(),
                null,
                new GyroscopeImplementation(),
                new LauncherImplementation(),
                new MagnetometerImplementation(),
                new MapImplementation(),
                new OrientationSensorImplementation(),
                new PhoneDialerImplementation(),
                () => new SecureStorageImplementation(),
                new AvaeSemanticScreenReader(),
                new ShareImplementation(),
                new SmsImplementation(),
                new TextToSpeechImplementation(),
                new VibrationImplementation());
#endif
        }
    }
}
