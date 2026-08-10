using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Maui.Essentials;
using Avalonia.Input.Platform;
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
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;
#if MACOS
//using Microsoft.Maui.Platforms.MacOS.Essentials;
#endif
namespace Avae.Essentials
{
    public static class Extensions
    {
        class TopLevelStateManagerImplementation : IAvaloniaEssentialsPlatformProvider
        {
            TopLevel? _active;

            public event EventHandler? ActiveChanged;

            public void OnActivated(TopLevel topLevel)
            {
                if (_active == topLevel)
                    return;

                _active = topLevel;

                ActiveChanged?.Invoke(topLevel, EventArgs.Empty);
            }

            public TopLevel? GetActive()
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

            public TopLevel? GetTopLevel()
            {
                return GetActive();
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
            IPermissions permissions,
            IPhoneDialer phoneDialer,
            Func<ISecureStorage> secureStorage,
            ISemanticScreenReader semanticScreenReader,
            IShare share,
            ISms sms,
            ITextToSpeech textToSpeech,
            IVibration vibration)
        {
            var platformProvider = new TopLevelStateManagerImplementation();
            EssentialsDefaults.SetScreenshot(null, new Avalonia.Controls.Maui.Essentials.AvaloniaScreenshot(platformProvider));
            //EssentialsDefaults.SetFilePicker(null, new Avalonia.Controls.Maui.Essentials.AvaloniaFilePicker(platformProvider));
            //EssentialsDefaults.SetMediaPicker(null, new Avalonia.Controls.Maui.Essentials.AvaloniaMediaPicker(platformProvider));
            EssentialsDefaults.SetHapticFeedback(null, new Avalonia.Controls.Maui.Essentials.AvaloniaHapticFeedback());
            EssentialsDefaults.SetPreferences(null, new Avalonia.Controls.Maui.Essentials.AvaloniaPreferences());
            EssentialsDefaults.SetFileSystem(null, new Avalonia.Controls.Maui.Essentials.AvaloniaFileSystem());
            //EssentialsDefaults.SetWebAuthenticator(null, new Avalonia.Controls.Maui.Essentials.AvaloniaWebAuthenticator(platformProvider));



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
                null,
                new PhoneDialerImplementation(),
                () => new SecureStorageImplementation(),
                new SemanticScreenReaderImplementation(),
                new ShareImplementation(),
                new SmsImplementation(),
                new TextToSpeechImplementation(),
                new VibrationImplementation());
#endif
        }
    }
}
