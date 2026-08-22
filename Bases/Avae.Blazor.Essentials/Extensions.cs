using Append.Blazor.WebShare;
using BlazorNative.Core;
using BlazorNative.Device;
using KristofferStrube.Blazor.FileSystemAccess;
using KristofferStrube.Blazor.MediaCaptureStreams;
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
using PatrickJahr.Blazor.AsyncClipboard;
using Toolbelt.Blazor.Extensions.DependencyInjection;

namespace Avae.Blazor.Essentials;

public static class Extensions
{
    public static void RegisterBlazorEssentials(this IServiceCollection services)
    {
        services.AddFileSystemAccessService();
        services.AddWebShare();
        services.AddAsyncClipboardService();
        services.AddMediaDevicesService();
        services.AddSpeechSynthesis();
        services.AddSingleton<IMobileBridge, DevHostBridge>();
        services.AddBlazorNativeDevice();
        services.AddSingleton<VideoCaptureCoordinator>();
        services.TryAddScoped<BlazorSensors.Accelerometer>();
        services.TryAddScoped<BlazorSensors.Gyroscope>();
        services.TryAddScoped<BlazorSensors.Magnetometer>();
        services.TryAddScoped<BlazorSensors.AbsoluteOrientationSensor>();

        //services.SetDefaults(
        //  new BlazorAccelerometer(),
        //  AppActions.Current,
        //  AppInfo.Current,
        //  Barometer.Default,
        //  Battery.Default,
        //  new BlazorBrowser(),
        //  new BlazorClipboard(),
        //  Compass.Default,
        //  Connectivity.Current,
        //  Contacts.Default,
        //  DeviceDisplay.Current,
        //  DeviceInfo.Current,
        //  Email.Default,
        //  new BlazorFilePicker(),
        //  FileSystem.Current,
        //  Flashlight.Default,
        //  new BlazorGeocoding(),
        //  new BlazorGeolocation(),
        //  new BlazorGyroscope(),
        //  HapticFeedback.Default,
        //  new BlazorLauncher(),
        //  new BlazorMagnetometer(),
        //  Map.Default,
        //  new BlazorMediaPicker(),
        //  new BlazorOrientationSensor(),
        //  new BlazorPhoneDialer(),
        //  Preferences.Default,
        //  Screenshot.Default,
        //  () => new BlazorSecureStorage(),
        //  SemanticScreenReader.Default,
        //  new BlazorShare(),
        //  new BlazorSms(),
        //  new BlazorTextToSpeech(),
        //  Vibration.Default,
        //  WebAuthenticator.Default,
        //  null!
        //  //VersionTracking.Default
        //  );

        //services.RegisterEssentials(ServiceLifetime.Scoped);

        services.TryAddScoped<IAccelerometer, BlazorAccelerometer>();
        services.TryAddScoped<IAppActions>(_ => AppActions.Current);
        services.TryAddScoped<IAppInfo>(_ => AppInfo.Current);
        services.TryAddScoped<IBarometer>(_ => Barometer.Default);
        services.TryAddScoped<IBattery>(_ => Battery.Default);
        services.TryAddScoped<IBrowser, BlazorBrowser>();
        services.TryAddScoped<IClipboard, BlazorClipboard>();
        services.TryAddScoped<ICompass>(_ => Compass.Default);
        services.TryAddScoped<IConnectivity>(_ => Connectivity.Current);
        services.TryAddScoped<IContacts>(_ => Contacts.Default);
        services.TryAddScoped<IDeviceDisplay>(_ => DeviceDisplay.Current);
        services.TryAddScoped<IDeviceInfo>(_ => DeviceInfo.Current);
        services.TryAddScoped<IEmail>(_ => Email.Default);
        services.TryAddScoped<IFilePicker, BlazorFilePicker>();
        services.TryAddScoped<IFileSystem>(_ => FileSystem.Current);
        services.TryAddScoped<IFlashlight>(_ => Flashlight.Default);
        services.TryAddScoped<IGeocoding, BlazorGeocoding>();
        services.TryAddScoped<Microsoft.Maui.Devices.Sensors.IGeolocation, BlazorGeolocation>();
        services.TryAddScoped<IGyroscope, BlazorGyroscope>();
        services.TryAddScoped<IHapticFeedback>(_ => HapticFeedback.Default);
        services.TryAddScoped<ILauncher, BlazorLauncher>();
        services.TryAddScoped<IMagnetometer, BlazorMagnetometer>();
        services.TryAddScoped<IMap>(_ => Map.Default);
        services.TryAddScoped<IMediaPicker, BlazorMediaPicker>();
        services.TryAddScoped<IOrientationSensor, BlazorOrientationSensor>();
        services.TryAddScoped<IPhoneDialer, BlazorPhoneDialer>();
        services.TryAddScoped<IPreferences>(_ => Preferences.Default);
        services.TryAddScoped<IScreenshot>(_ => Screenshot.Default);
        services.TryAddScoped<Microsoft.Maui.Storage.ISecureStorage, BlazorSecureStorage>();
        services.TryAddScoped<ISemanticScreenReader>(_ => SemanticScreenReader.Default);
        services.TryAddScoped<IShare, BlazorShare>();
        services.TryAddScoped<ISms, BlazorSms>();
        services.TryAddScoped<ITextToSpeech, BlazorTextToSpeech>();
        services.TryAddScoped<IVibration>(_ => Vibration.Default);
        services.TryAddScoped<IVersionTracking>(_ => VersionTracking.Default);
        services.TryAddScoped<IWebAuthenticator>(_ => WebAuthenticator.Default);
    }
}
