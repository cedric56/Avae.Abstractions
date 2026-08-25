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
    private static Dictionary<string, string> localeToCountry = new()
    {
        { "af-ZA", "South Africa" },
        { "am-ET", "Ethiopia" },
        { "ar-AE", "United Arab Emirates" },
        { "ar-BH", "Bahrain" },
        { "ar-DZ", "Algeria" },
        { "ar-EG", "Egypt" },
        { "ar-IQ", "Iraq" },
        { "ar-JO", "Jordan" },
        { "ar-KW", "Kuwait" },
        { "ar-LB", "Lebanon" },
        { "ar-LY", "Libya" },
        { "ar-MA", "Morocco" },
        { "ar-OM", "Oman" },
        { "ar-QA", "Qatar" },
        { "ar-SA", "Saudi Arabia" },
        { "ar-SD", "Sudan" },
        { "ar-SY", "Syria" },
        { "ar-TN", "Tunisia" },
        { "ar-YE", "Yemen" },
        { "az-AZ", "Azerbaijan" },
        { "be-BY", "Belarus" },
        { "bg-BG", "Bulgaria" },
        { "bn-BD", "Bangladesh" },
        { "bn-IN", "India" },
        { "bs-BA", "Bosnia and Herzegovina" },
        { "ca-ES", "Spain" },
        { "cs-CZ", "Czech Republic" },
        { "cy-GB", "United Kingdom" },
        { "da-DK", "Denmark" },
        { "de-AT", "Austria" },
        { "de-CH", "Switzerland" },
        { "de-DE", "Germany" },
        { "de-LI", "Liechtenstein" },
        { "de-LU", "Luxembourg" },
        { "el-CY", "Cyprus" },
        { "el-GR", "Greece" },
        { "en-AU", "Australia" },
        { "en-BZ", "Belize" },
        { "en-CA", "Canada" },
        { "en-CB", "Caribbean" },
        { "en-GB", "United Kingdom" },
        { "en-IE", "Ireland" },
        { "en-IN", "India" },
        { "en-JM", "Jamaica" },
        { "en-NZ", "New Zealand" },
        { "en-PH", "Philippines" },
        { "en-TT", "Trinidad and Tobago" },
        { "es-US", "United States" },
        { "en-US", "United States" },
        { "en-ZA", "South Africa" },
        { "en-ZW", "Zimbabwe" },
        { "es-AR", "Argentina" },
        { "es-BO", "Bolivia" },
        { "es-CL", "Chile" },
        { "es-CO", "Colombia" },
        { "es-CR", "Costa Rica" },
        { "es-DO", "Dominican Republic" },
        { "es-EC", "Ecuador" },
        { "es-ES", "Spain" },
        { "es-GT", "Guatemala" },
        { "es-HN", "Honduras" },
        { "es-MX", "Mexico" },
        { "es-NI", "Nicaragua" },
        { "es-PA", "Panama" },
        { "es-PE", "Peru" },
        { "es-PR", "Puerto Rico" },
        { "es-PY", "Paraguay" },
        { "es-SV", "El Salvador" },
        { "es-UY", "Uruguay" },
        { "es-VE", "Venezuela" },
        { "et-EE", "Estonia" },
        { "eu-ES", "Spain" },
        { "fa-IR", "Iran" },
        { "fi-FI", "Finland" },
        { "fo-FO", "Faroe Islands" },
        { "fr-BE", "Belgium" },
        { "fr-CA", "Canada" },
        { "fr-CH", "Switzerland" },
        { "fr-FR", "France" },
        { "fr-LU", "Luxembourg" },
        { "fr-MC", "Monaco" },
        { "gl-ES", "Spain" },
        { "gu-IN", "India" },
        { "he-IL", "Israel" },
        { "hi-IN", "India" },
        { "hr-BA", "Bosnia and Herzegovina" },
        { "hr-HR", "Croatia" },
        { "hu-HU", "Hungary" },
        { "hy-AM", "Armenia" },
        { "id-ID", "Indonesia" },
        { "is-IS", "Iceland" },
        { "it-CH", "Switzerland" },
        { "it-IT", "Italy" },
        { "ja-JP", "Japan" },
        { "ka-GE", "Georgia" },
        { "kk-KZ", "Kazakhstan" },
        { "kn-IN", "India" },
        { "ko-KR", "South Korea" },
        { "kok-IN", "India" },
        { "ky-KG", "Kyrgyzstan" },
        { "lt-LT", "Lithuania" },
        { "lv-LV", "Latvia" },
        { "mi-NZ", "New Zealand" },
        { "mk-MK", "North Macedonia" },
        { "ml-IN", "India" },
        { "mn-MN", "Mongolia" },
        { "mr-IN", "India" },
        { "ms-BN", "Brunei Darussalam" },
        { "ms-MY", "Malaysia" },
        { "mt-MT", "Malta" },
        { "nb-NO", "Norway" },
        { "nl-BE", "Belgium" },
        { "nl-NL", "Netherlands" },
        { "nn-NO", "Norway" },
        { "pa-IN", "India" },
        { "pl-PL", "Poland" },
        { "pt-BR", "Brazil" },
        { "pt-PT", "Portugal" },
        { "ro-RO", "Romania" },
        { "ru-RU", "Russia" },
        { "sa-IN", "India" },
        { "sk-SK", "Slovakia" },
        { "sl-SI", "Slovenia" },
        { "sq-AL", "Albania" },
        { "sr-Cyrl-BA", "Bosnia and Herzegovina" },
        { "sr-Cyrl-CS", "Serbia and Montenegro" },
        { "sr-Cyrl-ME", "Montenegro" },
        { "sr-Cyrl-RS", "Serbia" },
        { "sr-Latn-BA", "Bosnia and Herzegovina" },
        { "sr-Latn-CS", "Serbia and Montenegro" },
        { "sr-Latn-ME", "Montenegro" },
        { "sr-Latn-RS", "Serbia" },
        { "sv-FI", "Finland" },
        { "sv-SE", "Sweden" },
        { "sw-KE", "Kenya" },
        { "syr-SY", "Syria" },
        { "ta-IN", "India" },
        { "te-IN", "India" },
        { "th-TH", "Thailand" },
        { "tr-TR", "Turkey" },
        { "tt-RU", "Russia" },
        { "uk-UA", "Ukraine" },
        { "ur-PK", "Pakistan" },
        { "uz-Cyrl-UZ", "Uzbekistan" },
        { "uz-Latn-UZ", "Uzbekistan" },
        { "vi-VN", "Vietnam" },
        { "xh-ZA", "South Africa" },
        { "zh-CN", "China" },
        { "zh-HK", "Hong Kong" },
        { "zh-MO", "Macau" },
        { "zh-SG", "Singapore" },
        { "zh-TW", "Taiwan" },
        { "zu-ZA", "South Africa" }
    };

    public static string? GetCountry(string? language)
    {
        localeToCountry.TryGetValue(language ?? "en", out var country);
        return country;
    }

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
        Func<IVersionTracking> versionTracking, 
        ServiceLifetime lifetime = ServiceLifetime.Singleton)
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
        EssentialsAccessors.SetVersionTracking(null, versionTracking?.Invoke());

        services.RegisterEssentials(lifetime);
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

    public static Task<Stream> OpenReadAsync(this FileBase file, bool overridesMauiPlatform = true)
    {
        if (file == null) throw new ArgumentNullException(nameof(file));
        if (file is IAvaeFileResult avaeFileResult)
            return avaeFileResult.OpenFileStreamAsync();
        return file.OpenReadAsync();
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
