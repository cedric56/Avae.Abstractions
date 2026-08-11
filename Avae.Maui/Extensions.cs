using Avae.Abstractions;
using Avae.Services;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using UXDivers.Popups.Maui;

namespace Avae.Maui
{
    public static class Extensions
    {
        public static MauiAppBuilder ConfigureIocContainer<TApp>(this MauiAppBuilder builder,
            Action<IIocContainer>? configure = null,
            Action<ILoggingBuilder>? build = null)
            where TApp : Application
        {
            builder.Services.TryAddSingleton<IAccelerometer>(Accelerometer.Default);
            builder.Services.TryAddSingleton<IAppActions>(AppActions.Current);
            builder.Services.TryAddSingleton<IAppInfo>(AppInfo.Current);
            builder.Services.TryAddSingleton<IBarometer>(Barometer.Default);
            builder.Services.TryAddSingleton<IBattery>(Battery.Default);
            builder.Services.TryAddSingleton<IBrowser>(Browser.Default);
            builder.Services.TryAddSingleton<IClipboard>(Clipboard.Default);
            builder.Services.TryAddSingleton<ICompass>(Compass.Default);
            builder.Services.TryAddSingleton<IConnectivity>(Connectivity.Current);
            builder.Services.TryAddSingleton<IContacts>(Microsoft.Maui.ApplicationModel.Communication.Contacts.Default);
            builder.Services.TryAddSingleton<IDeviceDisplay>(DeviceDisplay.Current);
            builder.Services.TryAddSingleton<IDeviceInfo>(DeviceInfo.Current);
            builder.Services.TryAddSingleton<IEmail>(Email.Default);
            builder.Services.TryAddSingleton<IFilePicker>(FilePicker.Default);
            builder.Services.TryAddSingleton<IFlashlight>(Flashlight.Default);
            builder.Services.TryAddSingleton<IGeocoding>(Geocoding.Default);
            builder.Services.TryAddSingleton<IGeolocation>(Geolocation.Default);
            builder.Services.TryAddSingleton<IGyroscope>(Gyroscope.Default);
            builder.Services.TryAddSingleton<IHapticFeedback>(HapticFeedback.Default);
            builder.Services.TryAddSingleton<ILauncher>(Launcher.Default);
            builder.Services.TryAddSingleton<IMagnetometer>(Magnetometer.Default);
            builder.Services.TryAddSingleton<IMap>(Map.Default);
            builder.Services.TryAddSingleton<IMediaPicker>(MediaPicker.Default);
            builder.Services.TryAddSingleton<IOrientationSensor>(OrientationSensor.Default);
            builder.Services.TryAddSingleton<IPhoneDialer>(PhoneDialer.Default);
            builder.Services.TryAddSingleton<ISecureStorage>(SecureStorage.Default);
            builder.Services.TryAddSingleton<ISemanticScreenReader>(SemanticScreenReader.Default);
            builder.Services.TryAddSingleton<IShare>(Share.Default);
            builder.Services.TryAddSingleton<ISms>(Sms.Default);
            builder.Services.TryAddSingleton<ITextToSpeech>(TextToSpeech.Default);
            builder.Services.TryAddSingleton<IVibration>(Vibration.Default);
            builder.Services.TryAddSingleton<IWebAuthenticator>(WebAuthenticator.Default);



            //WindowsToastNotifyApi.Toast.Initialize("test", "here");

            builder.UseUXDiversPopups();
            builder.Services.AddSingleton<IIocContainer>(sp => new IocContainer(GetConfiguration(sp), false));
            builder.Services.AddSingleton<IIocConfiguration>(sp => new IocConfiguration(sp, () => (IocContainer)sp.GetRequiredService<IIocContainer>(), configure));
            builder.Services.AddTransient<Router>(sp => new Router(sp));
            builder.Services.AddSingleton<IDialogService>(GetConfiguration);
            builder.Services.AddSingleton<IContentDialogService>(GetConfiguration);
            builder.Services.AddSingleton<ITaskDialogService>(GetConfiguration);
            builder.Services.AddSingleton<ISystemNotificationService>(GetConfiguration);
            builder.Services.AddSingleton<INotificationService>(GetConfiguration);
            builder.Services.AddSingleton<IRequestedThemeService>(GetConfiguration);
            builder.Services.AddSingleton<ILogger>(LoggerFactory.Create(builder =>
            {
                build?.Invoke(builder);

            }).CreateLogger<TApp>());
            return builder;

            IocConfiguration GetConfiguration(IServiceProvider provider)
            {
                return (IocConfiguration)provider.GetRequiredService<IIocConfiguration>();
            }
        }
    }
}
