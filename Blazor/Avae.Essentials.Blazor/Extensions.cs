using Append.Blazor.WebShare;
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

namespace Avae.Essentials.Blazor;

public static class Extensions
{
    public static void RegisterBlazorEssentials(this IServiceCollection services)
    {
#if WEB
        throw new Exception("here");
#endif

        services.AddFileSystemAccessService();
        services.AddWebShare();
        services.AddAsyncClipboardService();
        services.AddMediaDevicesService();
        services.AddSpeechSynthesis();
        services.AddSingleton<VideoCaptureCoordinator>();

        services.TryAddScoped<IAccelerometer>(_ => Accelerometer.Default);
        services.TryAddScoped<IAppActions>(_ => AppActions.Current);
        services.TryAddScoped<IAppInfo>(_ => AppInfo.Current);
        services.TryAddScoped<IBarometer>(_ => Barometer.Default);
        services.TryAddScoped<IBattery>(_ => Battery.Default);
        services.TryAddScoped<IBrowser>(_ => Browser.Default);
        services.TryAddScoped<IClipboard, BlazorClipboard>();
        services.TryAddScoped<ICompass>(_ => Compass.Default);
        services.TryAddScoped<IConnectivity>(_ => Connectivity.Current);
        services.TryAddScoped<IContacts>(_ => Contacts.Default);
        services.TryAddScoped<IDeviceDisplay>(_ => DeviceDisplay.Current);
        services.TryAddScoped<IDeviceInfo>(_ => DeviceInfo.Current);
        services.TryAddScoped<IEmail>(_ => Email.Default);
        services.TryAddScoped<IFilePicker, BlazorFilePicker>();
        services.TryAddScoped<IFlashlight>(_ => Flashlight.Default);
        services.TryAddScoped<IGeocoding>(_ => Geocoding.Default);
        services.TryAddScoped<IGeolocation>(_ => Geolocation.Default);
        services.TryAddScoped<IGyroscope>(_ => Gyroscope.Default);
        services.TryAddScoped<IHapticFeedback>(_ => HapticFeedback.Default);
        services.TryAddScoped<ILauncher, BlazorLauncher>();
        services.TryAddScoped<IMagnetometer>(_ => Magnetometer.Default);
        services.TryAddScoped<IMap>(_ => Map.Default);
        services.TryAddScoped<IMediaPicker, BlazorMediaPicker>();
        services.TryAddScoped<IOrientationSensor>(_ => OrientationSensor.Default);
        services.TryAddScoped<IPhoneDialer>(_ => PhoneDialer.Default);
        services.TryAddScoped<ISecureStorage>(_ => SecureStorage.Default);
        services.TryAddScoped<ISemanticScreenReader>(_ => SemanticScreenReader.Default);
        services.TryAddScoped<IShare, BlazorShare>();
        services.TryAddScoped<ISms>(_ => Sms.Default);
        services.TryAddScoped<ITextToSpeech, BlazorTextToSpeech>();
        services.TryAddScoped<IVibration>(_ => Vibration.Default);
        services.TryAddScoped<IWebAuthenticator>(_ => WebAuthenticator.Default);
    }
}
