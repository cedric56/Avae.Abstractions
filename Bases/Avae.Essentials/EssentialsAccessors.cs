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
using System.Runtime.CompilerServices;

namespace Avae.Essentials;

/// <summary>
/// Installs Avalonia implementations into the Microsoft.Maui.Essentials static facades.
/// The facades only expose internal SetDefault/SetCurrent hooks, so these accessors use
/// <see cref="UnsafeAccessorAttribute"/> instead of MAUI internals.
/// </summary>
public static class EssentialsAccessors
{
    [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
    [return: UnsafeAccessorType("Microsoft.Maui.Media.Locale, Microsoft.Maui.Essentials")]
    public extern static object CreateLocale(string language, string country, string name, string id);

    [UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = "SetDefault")]
    public static extern void SetAccelerometer(
        [UnsafeAccessorType("Microsoft.Maui.Devices.Sensors.Accelerometer, Microsoft.Maui.Essentials")] object? facade,
        IAccelerometer? implementation);

    [UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = "SetCurrent")]
    public static extern void SetAppActions(
    [UnsafeAccessorType("Microsoft.Maui.ApplicationModel.AppActions, Microsoft.Maui.Essentials")] object? facade,
    IAppActions? implementation);

    [UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = "SetCurrent")]
    public static extern void SetAppInfo(
    [UnsafeAccessorType("Microsoft.Maui.ApplicationModel.AppInfo, Microsoft.Maui.Essentials")] object? facade,
    IAppInfo? implementation);

    [UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = "SetDefault")]
    public static extern void SetBarometer(
    [UnsafeAccessorType("Microsoft.Maui.Devices.Sensors.Barometer, Microsoft.Maui.Essentials")] object? facade,
    IBarometer? implementation);

    [UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = "SetDefault")]
    public static extern void SetBattery(
    [UnsafeAccessorType("Microsoft.Maui.Devices.Battery, Microsoft.Maui.Essentials")] object? facade,
    IBattery? implementation);

    [UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = "SetDefault")]
    public static extern void SetBrowser(
    [UnsafeAccessorType("Microsoft.Maui.ApplicationModel.Browser, Microsoft.Maui.Essentials")] object? facade,
    IBrowser? implementation);

    [UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = "SetDefault")]
    public static extern void SetClipboard(
    [UnsafeAccessorType("Microsoft.Maui.ApplicationModel.DataTransfer.Clipboard, Microsoft.Maui.Essentials")] object? facade,
    IClipboard? implementation);

    [UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = "SetDefault")]
    public static extern void SetCompass(
    [UnsafeAccessorType("Microsoft.Maui.Devices.Sensors.Compass, Microsoft.Maui.Essentials")] object? facade,
    ICompass? implementation);

    [UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = "SetCurrent")]
    public static extern void SetConnectivity(
    [UnsafeAccessorType("Microsoft.Maui.Networking.Connectivity, Microsoft.Maui.Essentials")] object? facade,
    IConnectivity? implementation);

    [UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = "SetDefault")]
    public static extern void SetContacts(
    [UnsafeAccessorType("Microsoft.Maui.ApplicationModel.Communication.Contacts, Microsoft.Maui.Essentials")] object? facade,
    IContacts? implementation);

    [UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = "SetCurrent")]
    public static extern void SetDeviceDisplay(
    [UnsafeAccessorType("Microsoft.Maui.Devices.DeviceDisplay, Microsoft.Maui.Essentials")] object? facade,
    IDeviceDisplay? implementation);

    [UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = "SetCurrent")]
    public static extern void SetDeviceInfo(
    [UnsafeAccessorType("Microsoft.Maui.Devices.DeviceInfo, Microsoft.Maui.Essentials")] object? facade,
    IDeviceInfo? implementation);

    [UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = "SetDefault")]
    public static extern void SetEmail(
    [UnsafeAccessorType("Microsoft.Maui.ApplicationModel.Communication.Email, Microsoft.Maui.Essentials")] object? facade,
    IEmail? implementation);

    [UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = "SetDefault")]
    public static extern void SetFlashlight(
    [UnsafeAccessorType("Microsoft.Maui.Devices.Flashlight, Microsoft.Maui.Essentials")] object? facade,
    IFlashlight? implementation);

    [UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = "SetDefault")]
    public static extern void SetVersionTracking(
    [UnsafeAccessorType("Microsoft.Maui.ApplicationModel.VersionTracking, Microsoft.Maui.Essentials")] object? facade,
    IVersionTracking? implementation);

    [UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = "SetCurrent")]
    public static extern void SetGeocoding(
        [UnsafeAccessorType("Microsoft.Maui.Devices.Sensors.Geocoding, Microsoft.Maui.Essentials")] object? facade,
        IGeocoding? implementation);

    [UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = "SetDefault")]
    public static extern void SetGeocolation(
        [UnsafeAccessorType("Microsoft.Maui.Devices.Sensors.Geolocation, Microsoft.Maui.Essentials")] object? facade,
        IGeolocation? implementation);

    [UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = "SetDefault")]
    public static extern void SetGyroscope(
    [UnsafeAccessorType("Microsoft.Maui.Devices.Sensors.Gyroscope, Microsoft.Maui.Essentials")] object? facade,
    IGyroscope? implementation);

    [UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = "SetDefault")]
    public static extern void SetLauncher(
    [UnsafeAccessorType("Microsoft.Maui.ApplicationModel.Launcher, Microsoft.Maui.Essentials")] object? facade,
    ILauncher? implementation);

    [UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = "SetDefault")]
    public static extern void SetMagnetometer(
    [UnsafeAccessorType("Microsoft.Maui.Devices.Sensors.Magnetometer, Microsoft.Maui.Essentials")] object? facade,
    IMagnetometer? implementation);

    [UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = "SetDefault")]
    public static extern void SetMap(
    [UnsafeAccessorType("Microsoft.Maui.ApplicationModel.Map, Microsoft.Maui.Essentials")] object? facade,
    IMap? implementation);

    [UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = "SetDefault")]
    public static extern void SetOrientationSensor(
        [UnsafeAccessorType("Microsoft.Maui.Devices.Sensors.OrientationSensor, Microsoft.Maui.Essentials")] object? facade,
        IOrientationSensor? implementation);
    //null,

    [UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = "SetDefault")]
    public static extern void SetPhoneDialer(
    [UnsafeAccessorType("Microsoft.Maui.ApplicationModel.Communication.PhoneDialer, Microsoft.Maui.Essentials")] object? facade,
    IPhoneDialer? implementation);

    [UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = "SetDefault")]
    public static extern void SetSecureStorage(
    [UnsafeAccessorType("Microsoft.Maui.Storage.SecureStorage, Microsoft.Maui.Essentials")] object? facade,
    ISecureStorage? implementation);

    [UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = "SetDefault")]
    public static extern void SetSemanticScreenReader(
    [UnsafeAccessorType("Microsoft.Maui.Accessibility.SemanticScreenReader, Microsoft.Maui.Essentials")] object? facade,
    ISemanticScreenReader? implementation);

    [UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = "SetDefault")]
    public static extern void SetShare(
    [UnsafeAccessorType("Microsoft.Maui.ApplicationModel.DataTransfer.Share, Microsoft.Maui.Essentials")] object? facade,
    IShare? implementation);

    [UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = "SetDefault")]
    public static extern void SetSms(
    [UnsafeAccessorType("Microsoft.Maui.ApplicationModel.Communication.Sms, Microsoft.Maui.Essentials")] object? facade,
    ISms? implementation);

    [UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = "SetDefault")]
    public static extern void SetTextToSpeech(
    [UnsafeAccessorType("Microsoft.Maui.Media.TextToSpeech, Microsoft.Maui.Essentials")] object? facade,
    ITextToSpeech? implementation);

    [UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = "SetDefault")]
    public static extern void SetVibration(
    [UnsafeAccessorType("Microsoft.Maui.Devices.Vibration, Microsoft.Maui.Essentials")] object? facade,
    IVibration? implementation);

    [UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = "SetDefault")]
    public static extern void SetFilePicker(
        [UnsafeAccessorType("Microsoft.Maui.Storage.FilePicker, Microsoft.Maui.Essentials")] object? facade,
        IFilePicker? implementation);

    [UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = "SetCurrent")]
    public static extern void SetFileSystem(
        [UnsafeAccessorType("Microsoft.Maui.Storage.FileSystem, Microsoft.Maui.Essentials")] object? facade,
        IFileSystem? implementation);

    [UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = "SetDefault")]
    public static extern void SetHapticFeedback(
        [UnsafeAccessorType("Microsoft.Maui.Devices.HapticFeedback, Microsoft.Maui.Essentials")] object? facade,
        IHapticFeedback? implementation);

    [UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = "SetDefault")]
    public static extern void SetMediaPicker(
        [UnsafeAccessorType("Microsoft.Maui.Media.MediaPicker, Microsoft.Maui.Essentials")] object? facade,
        IMediaPicker? implementation);

    [UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = "SetDefault")]
    public static extern void SetPreferences(
        [UnsafeAccessorType("Microsoft.Maui.Storage.Preferences, Microsoft.Maui.Essentials")] object? facade,
        IPreferences? implementation);

    [UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = "SetDefault")]
    public static extern void SetScreenshot(
        [UnsafeAccessorType("Microsoft.Maui.Media.Screenshot, Microsoft.Maui.Essentials")] object? facade,
        IScreenshot? implementation);

    [UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = "SetDefault")]
    public static extern void SetWebAuthenticator(
        [UnsafeAccessorType("Microsoft.Maui.Authentication.WebAuthenticator, Microsoft.Maui.Essentials")] object? facade,
        IWebAuthenticator? implementation);

    const string FallbackContentType = "application/octet-stream";
    public static string ResolveContentType(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            return FallbackContentType;

        var index = fileName.LastIndexOf('.');
        if (index < 0)
            return FallbackContentType;

        var extension = fileName[index..];
        return s_extensionMap.TryGetValue(extension, out var contentType)
            ? contentType
            : FallbackContentType;
    }
    // The following dictionary is derived from
    // Microsoft.AspNetCore.StaticFiles.FileExtensionContentTypeProvider's default mappings.
    //
    //   Source:    https://github.com/dotnet/aspnetcore/blob/main/src/Middleware/StaticFiles/src/FileExtensionContentTypeProvider.cs
    //   Copyright: (c) .NET Foundation and Contributors. All rights reserved.
    //   License:   Apache License, Version 2.0 — http://www.apache.org/licenses/LICENSE-2.0
    //
    // The full attribution notice and the Apache-2.0 license text required by section 4
    // of the license are reproduced in THIRD-PARTY-NOTICES.txt at the root of this repository.
    // When redistributing this assembly, that notice file must accompany the distribution.
    //
    // Why vendored (snapshot, not a dependency):
    //  - Microsoft.Maui.Essentials' portable build throws NotImplementedInReferenceAssemblyException
    //    from FileBase.PlatformGetContentType, so we cannot defer to MAUI here.
    //  - Taking a runtime dependency on Microsoft.AspNetCore.StaticFiles for a control library
    //    would pull ASP.NET Core into every consuming app, including non-server scenarios
    //    (desktop, Browser), which is not appropriate for an Avalonia/MAUI compatibility shim.
    //  - The mapping list is stable and rarely-changing; an inline copy keeps this library
    //    self-contained at the cost of being a point-in-time snapshot.
    //
    // Caveat: this table can drift from upstream over time. If a new common type is missing
    // (e.g. a future image/video format), re-sync from the link above. Files with extensions
    // not in this table fall back to "application/octet-stream".
    static readonly Dictionary<string, string> s_extensionMap = new(StringComparer.OrdinalIgnoreCase)
    {
        { ".323", "text/h323" },
        { ".3g2", "video/3gpp2" },
        { ".3gp2", "video/3gpp2" },
        { ".3gp", "video/3gpp" },
        { ".3gpp", "video/3gpp" },
        { ".aac", "audio/aac" },
        { ".aaf", "application/octet-stream" },
        { ".aca", "application/octet-stream" },
        { ".accdb", "application/msaccess" },
        { ".accde", "application/msaccess" },
        { ".accdt", "application/msaccess" },
        { ".acx", "application/internet-property-stream" },
        { ".adt", "audio/vnd.dlna.adts" },
        { ".adts", "audio/vnd.dlna.adts" },
        { ".afm", "application/octet-stream" },
        { ".ai", "application/postscript" },
        { ".aif", "audio/x-aiff" },
        { ".aifc", "audio/aiff" },
        { ".aiff", "audio/aiff" },
        { ".appcache", "text/cache-manifest" },
        { ".application", "application/x-ms-application" },
        { ".art", "image/x-jg" },
        { ".asd", "application/octet-stream" },
        { ".asf", "video/x-ms-asf" },
        { ".asi", "application/octet-stream" },
        { ".asm", "text/plain" },
        { ".asr", "video/x-ms-asf" },
        { ".asx", "video/x-ms-asf" },
        { ".atom", "application/atom+xml" },
        { ".au", "audio/basic" },
        { ".avi", "video/x-msvideo" },
        { ".avif", "image/avif" },
        { ".axs", "application/olescript" },
        { ".bas", "text/plain" },
        { ".bcpio", "application/x-bcpio" },
        { ".bin", "application/octet-stream" },
        { ".bmp", "image/bmp" },
        { ".c", "text/plain" },
        { ".cab", "application/vnd.ms-cab-compressed" },
        { ".calx", "application/vnd.ms-office.calx" },
        { ".cat", "application/vnd.ms-pki.seccat" },
        { ".cdf", "application/x-cdf" },
        { ".chm", "application/octet-stream" },
        { ".class", "application/x-java-applet" },
        { ".clp", "application/x-msclip" },
        { ".cmx", "image/x-cmx" },
        { ".cnf", "text/plain" },
        { ".cod", "image/cis-cod" },
        { ".cpio", "application/x-cpio" },
        { ".cpp", "text/plain" },
        { ".crd", "application/x-mscardfile" },
        { ".crl", "application/pkix-crl" },
        { ".crt", "application/x-x509-ca-cert" },
        { ".csh", "application/x-csh" },
        { ".css", "text/css" },
        { ".csv", "text/csv" },
        { ".cur", "application/octet-stream" },
        { ".dcr", "application/x-director" },
        { ".deploy", "application/octet-stream" },
        { ".der", "application/x-x509-ca-cert" },
        { ".dib", "image/bmp" },
        { ".dir", "application/x-director" },
        { ".disco", "text/xml" },
        { ".dlm", "text/dlm" },
        { ".doc", "application/msword" },
        { ".docm", "application/vnd.ms-word.document.macroEnabled.12" },
        { ".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
        { ".dot", "application/msword" },
        { ".dotm", "application/vnd.ms-word.template.macroEnabled.12" },
        { ".dotx", "application/vnd.openxmlformats-officedocument.wordprocessingml.template" },
        { ".dsp", "application/octet-stream" },
        { ".dtd", "text/xml" },
        { ".dvi", "application/x-dvi" },
        { ".dvr-ms", "video/x-ms-dvr" },
        { ".dwf", "drawing/x-dwf" },
        { ".dwp", "application/octet-stream" },
        { ".dxr", "application/x-director" },
        { ".eml", "message/rfc822" },
        { ".emz", "application/octet-stream" },
        { ".eot", "application/vnd.ms-fontobject" },
        { ".eps", "application/postscript" },
        { ".etx", "text/x-setext" },
        { ".evy", "application/envoy" },
        { ".exe", "application/vnd.microsoft.portable-executable" },
        { ".fdf", "application/vnd.fdf" },
        { ".fif", "application/fractals" },
        { ".fla", "application/octet-stream" },
        { ".flr", "x-world/x-vrml" },
        { ".flv", "video/x-flv" },
        { ".gif", "image/gif" },
        { ".gtar", "application/x-gtar" },
        { ".gz", "application/x-gzip" },
        { ".h", "text/plain" },
        { ".hdf", "application/x-hdf" },
        { ".hdml", "text/x-hdml" },
        { ".hhc", "application/x-oleobject" },
        { ".hhk", "application/octet-stream" },
        { ".hhp", "application/octet-stream" },
        { ".hlp", "application/winhlp" },
        { ".hqx", "application/mac-binhex40" },
        { ".hta", "application/hta" },
        { ".htc", "text/x-component" },
        { ".htm", "text/html" },
        { ".html", "text/html" },
        { ".htt", "text/webviewhtml" },
        { ".hxt", "text/html" },
        { ".ical", "text/calendar" },
        { ".icalendar", "text/calendar" },
        { ".ico", "image/x-icon" },
        { ".ics", "text/calendar" },
        { ".ief", "image/ief" },
        { ".ifb", "text/calendar" },
        { ".iii", "application/x-iphone" },
        { ".inf", "application/octet-stream" },
        { ".ins", "application/x-internet-signup" },
        { ".isp", "application/x-internet-signup" },
        { ".IVF", "video/x-ivf" },
        { ".jar", "application/java-archive" },
        { ".java", "application/octet-stream" },
        { ".jck", "application/liquidmotion" },
        { ".jcz", "application/liquidmotion" },
        { ".jfif", "image/pjpeg" },
        { ".jpb", "application/octet-stream" },
        { ".jpe", "image/jpeg" },
        { ".jpeg", "image/jpeg" },
        { ".jpg", "image/jpeg" },
        { ".js", "text/javascript" },
        { ".json", "application/json" },
        { ".jsx", "text/jscript" },
        { ".latex", "application/x-latex" },
        { ".lit", "application/x-ms-reader" },
        { ".lpk", "application/octet-stream" },
        { ".lsf", "video/x-la-asf" },
        { ".lsx", "video/x-la-asf" },
        { ".lzh", "application/octet-stream" },
        { ".m13", "application/x-msmediaview" },
        { ".m14", "application/x-msmediaview" },
        { ".m1v", "video/mpeg" },
        { ".m2ts", "video/vnd.dlna.mpeg-tts" },
        { ".m3u", "audio/x-mpegurl" },
        { ".m4a", "audio/mp4" },
        { ".m4v", "video/mp4" },
        { ".man", "application/x-troff-man" },
        { ".manifest", "application/x-ms-manifest" },
        { ".map", "text/plain" },
        { ".markdown", "text/markdown" },
        { ".md", "text/markdown" },
        { ".mdb", "application/x-msaccess" },
        { ".mdp", "application/octet-stream" },
        { ".me", "application/x-troff-me" },
        { ".mht", "message/rfc822" },
        { ".mhtml", "message/rfc822" },
        { ".mid", "audio/mid" },
        { ".midi", "audio/mid" },
        { ".mix", "application/octet-stream" },
        { ".mjs", "text/javascript" },
        { ".mmf", "application/x-smaf" },
        { ".mno", "text/xml" },
        { ".mny", "application/x-msmoney" },
        { ".mov", "video/quicktime" },
        { ".movie", "video/x-sgi-movie" },
        { ".mp2", "video/mpeg" },
        { ".mp3", "audio/mpeg" },
        { ".mp4", "video/mp4" },
        { ".mp4v", "video/mp4" },
        { ".mpa", "video/mpeg" },
        { ".mpe", "video/mpeg" },
        { ".mpeg", "video/mpeg" },
        { ".mpg", "video/mpeg" },
        { ".mpp", "application/vnd.ms-project" },
        { ".mpv2", "video/mpeg" },
        { ".ms", "application/x-troff-ms" },
        { ".msi", "application/octet-stream" },
        { ".mso", "application/octet-stream" },
        { ".mvb", "application/x-msmediaview" },
        { ".mvc", "application/x-miva-compiled" },
        { ".nc", "application/x-netcdf" },
        { ".nsc", "video/x-ms-asf" },
        { ".nws", "message/rfc822" },
        { ".ocx", "application/octet-stream" },
        { ".oda", "application/oda" },
        { ".odc", "text/x-ms-odc" },
        { ".ods", "application/oleobject" },
        { ".oga", "audio/ogg" },
        { ".ogg", "video/ogg" },
        { ".ogv", "video/ogg" },
        { ".ogx", "application/ogg" },
        { ".one", "application/onenote" },
        { ".onea", "application/onenote" },
        { ".onetoc", "application/onenote" },
        { ".onetoc2", "application/onenote" },
        { ".onetmp", "application/onenote" },
        { ".onepkg", "application/onenote" },
        { ".osdx", "application/opensearchdescription+xml" },
        { ".otf", "font/otf" },
        { ".p10", "application/pkcs10" },
        { ".p12", "application/x-pkcs12" },
        { ".p7b", "application/x-pkcs7-certificates" },
        { ".p7c", "application/pkcs7-mime" },
        { ".p7m", "application/pkcs7-mime" },
        { ".p7r", "application/x-pkcs7-certreqresp" },
        { ".p7s", "application/pkcs7-signature" },
        { ".pbm", "image/x-portable-bitmap" },
        { ".pcx", "application/octet-stream" },
        { ".pcz", "application/octet-stream" },
        { ".pdf", "application/pdf" },
        { ".pfb", "application/octet-stream" },
        { ".pfm", "application/octet-stream" },
        { ".pfx", "application/x-pkcs12" },
        { ".pgm", "image/x-portable-graymap" },
        { ".pko", "application/vnd.ms-pki.pko" },
        { ".pma", "application/x-perfmon" },
        { ".pmc", "application/x-perfmon" },
        { ".pml", "application/x-perfmon" },
        { ".pmr", "application/x-perfmon" },
        { ".pmw", "application/x-perfmon" },
        { ".png", "image/png" },
        { ".pnm", "image/x-portable-anymap" },
        { ".pnz", "image/png" },
        { ".pot", "application/vnd.ms-powerpoint" },
        { ".potm", "application/vnd.ms-powerpoint.template.macroEnabled.12" },
        { ".potx", "application/vnd.openxmlformats-officedocument.presentationml.template" },
        { ".ppam", "application/vnd.ms-powerpoint.addin.macroEnabled.12" },
        { ".ppm", "image/x-portable-pixmap" },
        { ".pps", "application/vnd.ms-powerpoint" },
        { ".ppsm", "application/vnd.ms-powerpoint.slideshow.macroEnabled.12" },
        { ".ppsx", "application/vnd.openxmlformats-officedocument.presentationml.slideshow" },
        { ".ppt", "application/vnd.ms-powerpoint" },
        { ".pptm", "application/vnd.ms-powerpoint.presentation.macroEnabled.12" },
        { ".pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation" },
        { ".prf", "application/pics-rules" },
        { ".prm", "application/octet-stream" },
        { ".prx", "application/octet-stream" },
        { ".ps", "application/postscript" },
        { ".psd", "application/octet-stream" },
        { ".psm", "application/octet-stream" },
        { ".psp", "application/octet-stream" },
        { ".pub", "application/x-mspublisher" },
        { ".qt", "video/quicktime" },
        { ".qtl", "application/x-quicktimeplayer" },
        { ".qxd", "application/octet-stream" },
        { ".ra", "audio/x-pn-realaudio" },
        { ".ram", "audio/x-pn-realaudio" },
        { ".rar", "application/octet-stream" },
        { ".ras", "image/x-cmu-raster" },
        { ".rf", "image/vnd.rn-realflash" },
        { ".rgb", "image/x-rgb" },
        { ".rm", "application/vnd.rn-realmedia" },
        { ".rmi", "audio/mid" },
        { ".roff", "application/x-troff" },
        { ".rpm", "audio/x-pn-realaudio-plugin" },
        { ".rtf", "application/rtf" },
        { ".rtx", "text/richtext" },
        { ".scd", "application/x-msschedule" },
        { ".sct", "text/scriptlet" },
        { ".sea", "application/octet-stream" },
        { ".setpay", "application/set-payment-initiation" },
        { ".setreg", "application/set-registration-initiation" },
        { ".sgml", "text/sgml" },
        { ".sh", "application/x-sh" },
        { ".shar", "application/x-shar" },
        { ".sit", "application/x-stuffit" },
        { ".sldm", "application/vnd.ms-powerpoint.slide.macroEnabled.12" },
        { ".sldx", "application/vnd.openxmlformats-officedocument.presentationml.slide" },
        { ".smd", "audio/x-smd" },
        { ".smi", "application/octet-stream" },
        { ".smx", "audio/x-smd" },
        { ".smz", "audio/x-smd" },
        { ".snd", "audio/basic" },
        { ".snp", "application/octet-stream" },
        { ".spc", "application/x-pkcs7-certificates" },
        { ".spl", "application/futuresplash" },
        { ".spx", "audio/ogg" },
        { ".src", "application/x-wais-source" },
        { ".ssm", "application/streamingmedia" },
        { ".sst", "application/vnd.ms-pki.certstore" },
        { ".stl", "application/vnd.ms-pki.stl" },
        { ".sv4cpio", "application/x-sv4cpio" },
        { ".sv4crc", "application/x-sv4crc" },
        { ".svg", "image/svg+xml" },
        { ".svgz", "image/svg+xml" },
        { ".swf", "application/x-shockwave-flash" },
        { ".t", "application/x-troff" },
        { ".tar", "application/x-tar" },
        { ".tcl", "application/x-tcl" },
        { ".tex", "application/x-tex" },
        { ".texi", "application/x-texinfo" },
        { ".texinfo", "application/x-texinfo" },
        { ".tgz", "application/x-compressed" },
        { ".thmx", "application/vnd.ms-officetheme" },
        { ".thn", "application/octet-stream" },
        { ".tif", "image/tiff" },
        { ".tiff", "image/tiff" },
        { ".toc", "application/octet-stream" },
        { ".tr", "application/x-troff" },
        { ".trm", "application/x-msterminal" },
        { ".ts", "video/vnd.dlna.mpeg-tts" },
        { ".tsv", "text/tab-separated-values" },
        { ".ttc", "application/x-font-ttf" },
        { ".ttf", "application/x-font-ttf" },
        { ".tts", "video/vnd.dlna.mpeg-tts" },
        { ".txt", "text/plain" },
        { ".u32", "application/octet-stream" },
        { ".uls", "text/iuls" },
        { ".ustar", "application/x-ustar" },
        { ".vbs", "text/vbscript" },
        { ".vcf", "text/x-vcard" },
        { ".vcs", "text/plain" },
        { ".vdx", "application/vnd.ms-visio.viewer" },
        { ".vml", "text/xml" },
        { ".vsd", "application/vnd.visio" },
        { ".vss", "application/vnd.visio" },
        { ".vst", "application/vnd.visio" },
        { ".vsto", "application/x-ms-vsto" },
        { ".vsw", "application/vnd.visio" },
        { ".vsx", "application/vnd.visio" },
        { ".vtx", "application/vnd.visio" },
        { ".wasm", "application/wasm" },
        { ".wav", "audio/wav" },
        { ".wax", "audio/x-ms-wax" },
        { ".wbmp", "image/vnd.wap.wbmp" },
        { ".wcm", "application/vnd.ms-works" },
        { ".wdb", "application/vnd.ms-works" },
        { ".webm", "video/webm" },
        { ".webmanifest", "application/manifest+json" },
        { ".webp", "image/webp" },
        { ".wks", "application/vnd.ms-works" },
        { ".wm", "video/x-ms-wm" },
        { ".wma", "audio/x-ms-wma" },
        { ".wmd", "application/x-ms-wmd" },
        { ".wmf", "application/x-msmetafile" },
        { ".wml", "text/vnd.wap.wml" },
        { ".wmlc", "application/vnd.wap.wmlc" },
        { ".wmls", "text/vnd.wap.wmlscript" },
        { ".wmlsc", "application/vnd.wap.wmlscriptc" },
        { ".wmp", "video/x-ms-wmp" },
        { ".wmv", "video/x-ms-wmv" },
        { ".wmx", "video/x-ms-wmx" },
        { ".wmz", "application/x-ms-wmz" },
        { ".woff", "application/font-woff" },
        { ".woff2", "font/woff2" },
        { ".wps", "application/vnd.ms-works" },
        { ".wri", "application/x-mswrite" },
        { ".wrl", "x-world/x-vrml" },
        { ".wrz", "x-world/x-vrml" },
        { ".wsdl", "text/xml" },
        { ".wtv", "video/x-ms-wtv" },
        { ".wvx", "video/x-ms-wvx" },
        { ".x", "application/directx" },
        { ".xaf", "x-world/x-vrml" },
        { ".xaml", "application/xaml+xml" },
        { ".xap", "application/x-silverlight-app" },
        { ".xbap", "application/x-ms-xbap" },
        { ".xbm", "image/x-xbitmap" },
        { ".xdr", "text/plain" },
        { ".xht", "application/xhtml+xml" },
        { ".xhtml", "application/xhtml+xml" },
        { ".xla", "application/vnd.ms-excel" },
        { ".xlam", "application/vnd.ms-excel.addin.macroEnabled.12" },
        { ".xlc", "application/vnd.ms-excel" },
        { ".xlm", "application/vnd.ms-excel" },
        { ".xls", "application/vnd.ms-excel" },
        { ".xlsb", "application/vnd.ms-excel.sheet.binary.macroEnabled.12" },
        { ".xlsm", "application/vnd.ms-excel.sheet.macroEnabled.12" },
        { ".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
        { ".xlt", "application/vnd.ms-excel" },
        { ".xltm", "application/vnd.ms-excel.template.macroEnabled.12" },
        { ".xltx", "application/vnd.openxmlformats-officedocument.spreadsheetml.template" },
        { ".xlw", "application/vnd.ms-excel" },
        { ".xml", "text/xml" },
        { ".xof", "x-world/x-vrml" },
        { ".xpm", "image/x-xpixmap" },
        { ".xps", "application/vnd.ms-xpsdocument" },
        { ".xsd", "text/xml" },
        { ".xsf", "text/xml" },
        { ".xsl", "text/xml" },
        { ".xslt", "text/xml" },
        { ".xsn", "application/octet-stream" },
        { ".xtp", "application/octet-stream" },
        { ".xwd", "image/x-xwindowdump" },
        { ".z", "application/x-compress" },
        { ".zip", "application/x-zip-compressed" },
    };
}
