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
    public static void SetDefaults(this IServiceCollection services,
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
        EssentialsAccessors.SetScreenshot(null, screenshot);
        EssentialsAccessors.SetFilePicker(null, filepicker);
        EssentialsAccessors.SetMediaPicker(null, mediaPicker);
        EssentialsAccessors.SetHapticFeedback(null, hapticFeedback);
        EssentialsAccessors.SetPreferences(null, preferences);
        EssentialsAccessors.SetFileSystem(null, fileSystem);
        EssentialsAccessors.SetWebAuthenticator(null, webAuthenticator);
        EssentialsAccessors.SetAccelerometer(null, accelerometer);
        EssentialsAccessors.SetAppActions(null, appActions);
        EssentialsAccessors.SetAppInfo(null, appInfo);
        EssentialsAccessors.SetBarometer(null, barometer);
        EssentialsAccessors.SetBattery(null, battery);
        EssentialsAccessors.SetBrowser(null, browser);
        EssentialsAccessors.SetClipboard(null, clipboard);
        EssentialsAccessors.SetCompass(null, compass);
        EssentialsAccessors.SetConnectivity(null, connectivity);
        EssentialsAccessors.SetContacts(null, contacts);
        EssentialsAccessors.SetDeviceDisplay(null, deviceDisplay);
        EssentialsAccessors.SetDeviceInfo(null, deviceInfo);
        EssentialsAccessors.SetEmail(null, email);
        EssentialsAccessors.SetFlashlight(null, flashlight);
        EssentialsAccessors.SetGeocoding(null, geocoding);
        EssentialsAccessors.SetGeocolation(null, geolocation);
        EssentialsAccessors.SetGyroscope(null, gyroscope);
        EssentialsAccessors.SetLauncher(null, launcher);
        EssentialsAccessors.SetMagnetometer(null, magnetometer);
        EssentialsAccessors.SetMap(null, map);
        EssentialsAccessors.SetOrientationSensor(null, orientationSensor);
        EssentialsAccessors.SetPhoneDialer(null, phoneDialer);
        EssentialsAccessors.SetSecureStorage(null, secureStorage?.Invoke());
        EssentialsAccessors.SetSemanticScreenReader(null, semanticScreenReader);
        EssentialsAccessors.SetShare(null, share);
        EssentialsAccessors.SetSms(null, sms);
        EssentialsAccessors.SetTextToSpeech(null, textToSpeech);
        EssentialsAccessors.SetVibration(null, vibration);
    }

    public static void RegisterEssentials(this IServiceCollection services)
    {
        services.TryAddSingleton(Accelerometer.Default);
        services.TryAddSingleton(AppActions.Current);
        services.TryAddSingleton(AppInfo.Current);
        services.TryAddSingleton(Barometer.Default);
        services.TryAddSingleton(Battery.Default);
        services.TryAddSingleton(Browser.Default);
        services.TryAddSingleton(Clipboard.Default);
        services.TryAddSingleton(Compass.Default);
        services.TryAddSingleton(Connectivity.Current);
        services.TryAddSingleton(Contacts.Default);
        services.TryAddSingleton(DeviceDisplay.Current);
        services.TryAddSingleton(DeviceInfo.Current);
        services.TryAddSingleton(Email.Default);
        services.TryAddSingleton(FilePicker.Default);
        services.TryAddSingleton(Flashlight.Default);
        services.TryAddSingleton(Geocoding.Default);
        services.TryAddSingleton(Geolocation.Default);
        services.TryAddSingleton(Gyroscope.Default);
        services.TryAddSingleton(HapticFeedback.Default);
        services.TryAddSingleton(Launcher.Default);
        services.TryAddSingleton(Magnetometer.Default);
        services.TryAddSingleton(Map.Default);
        services.TryAddSingleton(MediaPicker.Default);
        services.TryAddSingleton(OrientationSensor.Default);
        services.TryAddSingleton(PhoneDialer.Default);
        services.TryAddSingleton(SecureStorage.Default);
        services.TryAddSingleton(SemanticScreenReader.Default);
        services.TryAddSingleton(Share.Default);
        services.TryAddSingleton(Sms.Default);
        services.TryAddSingleton(TextToSpeech.Default);
        services.TryAddSingleton(Vibration.Default);
        services.TryAddSingleton(WebAuthenticator.Default);
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
