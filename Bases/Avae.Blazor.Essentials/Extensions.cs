using Append.Blazor.WebShare;
using Avae.Essentials;
using BlazorNative.Core;
using BlazorNative.Device;
using KristofferStrube.Blazor.FileSystemAccess;
using KristofferStrube.Blazor.MediaCaptureStreams;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Maui.Accessibility;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.ApplicationModel.Communication;
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
    public static void UseBlazorEssentials(this IServiceCollection services)
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

        services.SetDefaults(
          new BlazorAccelerometer(),
          AppActions.Current,
          AppInfo.Current,
          Barometer.Default,
          Battery.Default,
          new BlazorBrowser(),
          new BlazorClipboard(),
          Compass.Default,
          Connectivity.Current,
          Contacts.Default,
          DeviceDisplay.Current,
          DeviceInfo.Current,
          Email.Default,
          new BlazorFilePicker(),
          FileSystem.Current,
          Flashlight.Default,
          new BlazorGeocoding(),
          Geolocation.Default,
          new BlazorGyroscope(),
          HapticFeedback.Default,
          new BlazorLauncher(),
          new BlazorMagnetometer(),
          Map.Default,
          new BlazorMediaPicker(),
          new BlazorOrientationSensor(),
          new BlazorPhoneDialer(),
          Preferences.Default,
          Screenshot.Default,
          () => new BlazorSecureStorage(),
          SemanticScreenReader.Default,
          new BlazorShare(),
          new BlazorSms(),
          new BlazorTextToSpeech(),
          Vibration.Default,
          WebAuthenticator.Default,
          null!,
          //VersionTracking.Default,
          ServiceLifetime.Scoped);
    }
}
