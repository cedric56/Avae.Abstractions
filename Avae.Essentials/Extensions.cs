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

namespace Avae.Essentials;

public static class Extensions
{
    public static void RegisterEssentials(this IServiceCollection services)
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
        services.TryAddSingleton<IContacts>(Contacts.Default);
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

    public static Task ComposeAsync(this IEmail email, IEnumerable<FileBase> files, EmailMessage message)
    {
        if (email is IAvaeEmail avae)
        {
            return avae.ComposeAsync(files, message);
        }
        else
        {
            var attachments = new List<EmailAttachment>();
            foreach (var file in files ?? [])
            {
                attachments.Add(new EmailAttachment(file.FullPath));
            }
            message.Attachments = attachments;
            return email.ComposeAsync(message);
        }
    }

    public static Task RequestAsync(this IShare share, string title, IEnumerable<FileBase> files)
    {
        if (share is IAvaeShare avae)
        {
            return avae.RequestAsync(title, files);
        }
        else
        {
            // Convert the enumerable to a list to avoid multiple enumeration and get accurate count
            var shareFiles = new List<ShareFile>(files.Count());

            foreach (var file in files)
            {
                // Use standard MAUI ShareFile for regular files
                shareFiles.Add(new ShareFile(file));
            }

            // Execute the native share request with the converted files
            return share.RequestAsync(new ShareMultipleFilesRequest()
            {
                Title = title,
                Files = shareFiles
            });
        }
    }
}
