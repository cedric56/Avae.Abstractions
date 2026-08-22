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
        IWebAuthenticator webAuthenticator,
        IVersionTracking versionTracking)
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
        EssentialsAccessors.SetVersionTracking(null, versionTracking);
    }

    public static void RegisterEssentials(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Singleton)
    {
        services.TryAdd(ServiceDescriptor.Describe(typeof(IAccelerometer), _ => Accelerometer.Default, lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(IAppActions), _ => AppActions.Current, lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(IAppInfo), _ => AppInfo.Current, lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(IBarometer), _ => Barometer.Default, lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(IBattery), _ => Battery.Default, lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(IBrowser), _ => Browser.Default, lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(IClipboard), _ => Clipboard.Default, lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(ICompass), _ => Compass.Default, lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(IConnectivity), _ => Connectivity.Current, lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(IContacts), _ => Contacts.Default, lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(IDeviceDisplay), _ => DeviceDisplay.Current, lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(IDeviceInfo), _ => DeviceInfo.Current, lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(IEmail), _ => Email.Default, lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(IFilePicker), _ => FilePicker.Default, lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(IFileSystem), _ => FileSystem.Current, lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(IFlashlight), _ => Flashlight.Default, lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(IGeocoding), _ => Geocoding.Default, lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(IGeolocation), _ => Geolocation.Default, lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(IGyroscope), _ => Gyroscope.Default, lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(IHapticFeedback), _ => HapticFeedback.Default, lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(ILauncher), _ => Launcher.Default, lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(IMagnetometer), _ => Magnetometer.Default, lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(IMap), _ => Map.Default, lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(IMediaPicker), _ => MediaPicker.Default, lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(IOrientationSensor), _ => OrientationSensor.Default, lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(IPhoneDialer), _ => PhoneDialer.Default, lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(IPreferences), _ => Preferences.Default, lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(IScreenshot), _ => Screenshot.Default, lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(ISecureStorage), _ => SecureStorage.Default, lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(ISemanticScreenReader), _ => SemanticScreenReader.Default, lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(IShare), _ => Share.Default, lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(ISms), _ => Sms.Default, lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(ITextToSpeech), _ => TextToSpeech.Default, lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(IVibration), _ => Vibration.Default, lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(IVersionTracking), _ => VersionTracking.Default, lifetime));
        services.TryAdd(ServiceDescriptor.Describe(typeof(IWebAuthenticator), _ => WebAuthenticator.Default, lifetime));
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
