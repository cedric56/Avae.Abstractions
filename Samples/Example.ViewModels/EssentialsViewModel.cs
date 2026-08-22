using Avae.Essentials;
using Avae.Services;
using Avae.ViewModels;
using CommunityToolkit.Mvvm.Input;
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
using System.Web;

namespace Example.ViewModels
{
    public partial class EssentialsViewModel(
        INotificationService service,
        IAccelerometer accelerometer,
        IAppActions appActions,
        IAppInfo appInfo,
        IBarometer barometer,
        IBattery battery,
        IBrowser browser,
        //IClipboard clipboard,
        ICompass compass,
        IConnectivity connectivity,
        IContacts contacts,
        IDeviceDisplay deviceDisplay,
        IDeviceInfo deviceInfo,
        IEmail email,
        IFilePicker filePicker,
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
        ISecureStorage secureStorage,
        ISemanticScreenReader semanticScreenReader,
        IShare share,
        ISms sms,
        ITextToSpeech textToSpeech,
        IVibration vibration,
        IVersionTracking versionTracking
        //,
        //IWebAuthenticator webAuthenticator
        ) : IViewModelBase
    {
        bool IsSupportedInMauiPlatform()
        {
#if WINDOWS || ANDROID || MACCATALYST || IOS
            return true;
#else
            return false;
#endif
        }

        async Task<bool> CheckPermission(Permissions.BasePermission permission)
        {
            if (OperatingSystem.IsAndroid() || OperatingSystem.IsIOS())
            {
                if (PermissionStatus.Granted != await permission.CheckStatusAsync())
                {
                    if (PermissionStatus.Granted != await permission.RequestAsync())
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        [RelayCommand]
        public void AccelerometerCmd()
        {
            if (accelerometer.IsSupported)
            {
                if (accelerometer.IsMonitoring)
                {
                    accelerometer.Stop();
                    return;
                }

                accelerometer.Start(SensorSpeed.Default);
                accelerometer.ShakeDetected += (sender, args) =>
                {
                    service.Show("Shake", "");
                };
                accelerometer.ReadingChanged += (sender, args) =>
                {
                    service.Show(args.Reading.ToString(), "");
                };
            }
        }



        [RelayCommand]
        public async Task PickFileCmd()
        {
            var results = await filePicker.PickAsync();
            if (results != null)
            {
                service.Show(results.FileName, "");
            }
        }

        [RelayCommand(CanExecute = nameof(IsSupportedInMauiPlatform))]
        public async Task PickImageCmd()
        {
            var options = new PickOptions()
            {
                FileTypes = FilePickerFileType.Images
            };

            var results = await filePicker.PickAsync(options);
            if (results != null)
            {
                service.Show(results.FileName, "");
            }
        }

        [RelayCommand(CanExecute = nameof(IsSupportedInMauiPlatform))]
        public async Task PickPdfCmd()
        {
            var results = await filePicker.PickAsync(new PickOptions()
            {
                FileTypes = FilePickerFileType.Pdf

            }); if (results != null)
            {
                service.Show(results.FileName, "");
            }
        }

        public struct DevicePlatformEx
        {
            public static DevicePlatform Linux => DevicePlatform.Create(nameof(Linux));
            public static DevicePlatform Wasm => DevicePlatform.Create(nameof(Wasm));
        }



        [RelayCommand]
        public async Task PickCustomTypeCmd()
        {
            var options = new PickOptions
            {
                PickerTitle = "Please select a comic file",
                FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.iOS, new[] { "public.my.comic.extension" } }, // or general UTType values
                    { DevicePlatform.Android, new[] { "application/comics" } },
                    { DevicePlatform.WinUI, new[] { ".cbr", ".cbz" } },
                    { DevicePlatform.Tizen, new[] { "*/*" } },
                    { DevicePlatform.macOS, new[] { "cbr", "cbz" } }, // or general UTType values
                    { DevicePlatformEx.Wasm, new[] { ".cbr", ".cbz" } }, // or general UTType values
                    { DevicePlatformEx.Linux, new[] { ".cbr", ".cbz" } }, // or general UTType values
                })
            };
            var results = await filePicker.PickAsync(options);
            if (results != null)
            {
                service.Show(results.FileName, "");
            }
        }

        [RelayCommand]
        public async Task PickMultipleFilesCmd()
        {
            var results = await filePicker.PickMultipleAsync();
            if (results != null)
            {
                service.Show(results.FirstOrDefault()?.FileName ?? string.Empty, "");
            }
        }

        [RelayCommand]
        public async Task GoogleCmd()
        {
            try
            {
                
            }
            catch (TimeoutException)
            {
                AuthToken = string.Empty;
            }
            catch (TaskCanceledException)
            {
                AuthToken = string.Empty;
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Login canceled.");

                AuthToken = string.Empty;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed: {ex.Message}");

                AuthToken = string.Empty;
            }
        }

        string accessToken = string.Empty;

        public string AuthToken
        {
            get => accessToken;
            set { accessToken = value; }
        }

        //private class ResponderDecoder(string urlToken, string oauthCode, Dictionary<string, string> dico) : IWebAuthenticatorResponseDecoder
        //{
        //    readonly string oauthCode = oauthCode;
        //    readonly string urlToken = urlToken;
        //    readonly Dictionary<string, string> dico = dico;

        //    public IDictionary<string, string>? DecodeResponse(Uri uri)
        //    {
        //        var values = HttpUtility.ParseQueryString(uri.Query)[oauthCode];
        //        if (values != null)
        //            dico.Add(oauthCode, values);
        //        var tokenRequestBody = new FormUrlEncodedContent(dico);
        //        using var httpClient = new HttpClient();
        //        var tokenResponse = httpClient.PostAsync(urlToken, tokenRequestBody).Result;
        //        var tokenContent = tokenResponse.Content.ReadAsStringAsync().Result;
        //        var dict = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(tokenContent);
        //        return dict?.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToString() ?? string.Empty);
        //    }
        //}



        [RelayCommand]
        public void DeviceInfoCmd()
        {
            service.Show(deviceInfo.Platform.ToString(), "");
        }

        [RelayCommand]
        public async Task SmsCmdAsync()
        {
            if (!await this.CheckPermission(new Permissions.Sms()))
                return;

            var message = new SmsMessage("Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.",
                "0652760516");
            await sms.ComposeAsync(message);
        }


        [RelayCommand]
        public async Task MapCmdAsync()
        {
            if (!await this.CheckPermission(new Permissions.LocationWhenInUse()))
                return;

            var lo = await geolocation.GetLocationAsync();
            if (lo != null)
                await map.OpenAsync(lo, new MapLaunchOptions()
                {
                    NavigationMode = NavigationMode.Bicycling
                });

        }
        /// <summary>
        /// https://github.com/MicroSugarDeveloperOrg/Avalonia.WebView
        /// </summary>
        [RelayCommand]
        public async Task BrowserCmdAsync()
        {
            await browser.OpenAsync(new Uri("http://www.google.fr"),
                BrowserLaunchMode.SystemPreferred);
        }

        [RelayCommand]
        public void AppInfoCmd()
        {
            appInfo.ShowSettingsUI();
        }


        [RelayCommand]
        public async Task ConnectivityCmdAsync()
        {
            if (!await this.CheckPermission(new Permissions.NetworkState()))
                return;

            var profiles = connectivity.ConnectionProfiles;
            if (profiles != null)
            {

            }

            service.Show(connectivity.NetworkAccess.ToString(), "Message");
        }
        [RelayCommand]
        public void MagnetometerCmd()
        {
            if (magnetometer.IsSupported)
            {
                if (magnetometer.IsMonitoring)
                {
                    magnetometer.Stop();
                    return;
                }

                magnetometer.Start(SensorSpeed.Default);

                magnetometer.ReadingChanged += (sender, args) =>
                {
                    service.Show(args.Reading.ToString(), "");
                };
            }
        }

        [RelayCommand]
        public void BarometerCmd()
        {
            if (barometer.IsSupported)
            {
                if (barometer.IsMonitoring)
                {
                    barometer.Stop();
                    return;
                }

                barometer.Start(SensorSpeed.Default);
                barometer.ReadingChanged += (sender, args) =>
                {
                    service.Show(args.Reading.ToString(), "");
                };
            }
        }


        [RelayCommand]
        public void OrientationSensorCmd()
        {
            if (orientationSensor.IsSupported)
            {
                if (orientationSensor.IsMonitoring)
                {
                    orientationSensor.Stop();
                    return;
                }

                orientationSensor.Start(SensorSpeed.Default);
                orientationSensor.ReadingChanged += (sender, args) =>
                {
                    service.Show(args.Reading.ToString(), "");

                };
            }
        }

        [RelayCommand]
        public async Task VibrationCmdAsync()
        {
            if (!await this.CheckPermission(new Permissions.Vibrate()))
                return;

            if (vibration.IsSupported)
                vibration.Vibrate();

        }

        [RelayCommand]
        public async Task FlashOnCmd()
        {
            if (!await this.CheckPermission(new Permissions.Flashlight()))
                return;

            if (await flashlight.IsSupportedAsync())
                await flashlight.TurnOnAsync();
        }


        [RelayCommand]
        public async Task FlashOffCmd()
        {
            if (!await this.CheckPermission(new Permissions.Flashlight()))
                return;

            if (await flashlight.IsSupportedAsync())
                await flashlight.TurnOffAsync();
        }

        [RelayCommand]
        public void DialCmd()
        {
            if (phoneDialer.IsSupported)
                phoneDialer.Open("0652760516");
        }

        [RelayCommand]
        public async Task CapturePhotoCmd()
        {
            if (!await this.CheckPermission(new Permissions.Media()))
                return;

            if (true == mediaPicker.IsCaptureSupported)
            {
                var s = await mediaPicker.CapturePhotoAsync();
                if (s != null)
                {
                    service.Show(s.FullPath, "File");


                    using var stream = await s.OpenReadAsync();

                }
            }
        }

        [RelayCommand]
        public async Task CaptureVideoCmd()
        {
            if (!await this.CheckPermission(new Permissions.Media()))
                return;
            if (true == mediaPicker.IsCaptureSupported)
            {
                var s = await mediaPicker.CaptureVideoAsync();
                if (s != null)
                {
                    service.Show(s.FullPath, "File");

                }
            }
        }

        [RelayCommand]
        public void PreferencesCmd()
        {
            preferences.ContainsKey("");
            var result = preferences.Get("hello", string.Empty);
            service.Show(result, "Must be null");
            preferences.Set("hello", "world");
            result = preferences.Get("hello", string.Empty);
            service.Show(result, "Must be world");
            preferences.Remove("hello");
            result = preferences.Get("hello", string.Empty);
            service.Show(result, "Must be null");
        }

        [RelayCommand]
        public async Task PickPhotoCmd()
        {
            if (!await this.CheckPermission(new Permissions.Media()))
                return;

            var s = await mediaPicker.PickPhotosAsync();
            if (s.Count>0)
            {
                service.Show(s[0].FileName, "");
            }
        }

        [RelayCommand]
        public async Task ActionsCmd()
        {
            await appActions.SetAsync([new AppAction("1", "title")]);
        }

        [RelayCommand]
        public async Task PickVideoCmd()
        {
            if (!await this.CheckPermission(new Permissions.Media()))
                return;

            var s = await mediaPicker.PickVideosAsync();
            if (s.Count>0)
            {
                service.Show(s[0].FileName, "");
            }
        }

        [RelayCommand]
        public async Task LaunchMailCmd()
        {
            var recipient = "example@example.com";
            var subject = "Hello from Blazor!";
            var body = "This is a test email from my Blazor app.";

            // Build the mailto URI
            var mailtoUri = $"mailto:{recipient}?subject={Uri.EscapeDataString(subject)}&body={Uri.EscapeDataString(body)}";
            await launcher.OpenAsync(new Uri(mailtoUri));
        }

        [RelayCommand]
        public async Task LaunchBrowserCmd()
        {
            await launcher.OpenAsync(new Uri("https://github.com/xamarin/Essentials"));
        }

        [RelayCommand]
        public async Task BatteryCmd()
        {
            if (!await this.CheckPermission(new Permissions.Battery()))
                return;
            var level = battery.ChargeLevel;
            service.Show(battery.PowerSource.ToString(), "Message");
        }

        [RelayCommand]
        public async Task SpeechCmdAsync()
        {
            var voices = await textToSpeech.GetLocalesAsync();
            await textToSpeech.SpeakAsync("Bonjour Maya", new SpeechOptions()
            {
                Locale = voices.LastOrDefault(),
                Pitch = 1.0f,
                Volume = 1.0f
            });
        }


        [RelayCommand]
        public async Task ShareCmdAsync()
        {
            var results = await filePicker.PickMultipleAsync();
            await share.RequestAsync("title", results ?? []);
        }

        [RelayCommand]
        public async Task CompassCmdAsync()
        {
            if (!await this.CheckPermission(new Permissions.Sensors()))
                return;

            if (compass.IsSupported)
            {
                if (compass.IsMonitoring)
                {
                    compass.Stop();
                    return;
                }

                compass.Start(SensorSpeed.Default);

                compass.ReadingChanged += (sender, args) =>
                {
                    service.Show(args.Reading.HeadingMagneticNorth.ToString(), "Message");
                };
            }
        }

        [RelayCommand]
        public async Task ContactsCmd()
        {
            var values = await contacts.GetAllAsync();

            service.Show(values.FirstOrDefault()?.ToString() ?? string.Empty, "");

            //service.Show(contacts);
        }

        [RelayCommand]
        public void DeviceDisplayCmd()
        {
            deviceDisplay.MainDisplayInfoChanged += (sender, args) =>
            {
                service.Show(args.DisplayInfo.ToString(), "");
            };
            deviceDisplay.KeepScreenOn = true;
            service.Show(deviceDisplay.MainDisplayInfo.ToString(), "");
        }

        [RelayCommand]
        public async Task EmailCmd()
        {
            await email.ComposeAsync();
        }

        [RelayCommand]
        public async Task FileSystemCmd()
        {
            service.Show(fileSystem.CacheDirectory, "Cache directory");
        }

        [RelayCommand]
        public async Task GeocodeCmd()
        {
            try
            {
                var results = await geocoding.GetLocationsAsync("37 rue du lannic 56870 BADEN");
                foreach (var location in results)
                {
                    service.Show(location.ToString(), "");
                }
            }
            catch
            {

            }
        }

        [RelayCommand]
        public async Task GeolocationCmd()
        {
            var result = await geolocation.GetLocationAsync();
            if (result != null)
            {
                service.Show(result.ToString(), "");
            }
        }

        [RelayCommand]
        public void HapticCmd()
        {
            hapticFeedback.Perform(HapticFeedbackType.LongPress);
        }

        [RelayCommand]
        public void GyroscopeCmd()
        {
            if (gyroscope.IsSupported)
            {
                if (gyroscope.IsMonitoring)
                {
                    gyroscope.Stop();
                    return;
                }

                gyroscope.Start(SensorSpeed.UI);
                gyroscope.ReadingChanged += (sender, args) =>
                {
                    service.Show(args.Reading.ToString(), "");

                };
            }
        }

        [RelayCommand]
        public async Task ScreenshotCmd()
        {
            if (screenshot.IsCaptureSupported)
            {
                var result = await screenshot.CaptureAsync();
                using var stream = await result.OpenReadAsync();
                using var memoryStream = new MemoryStream();
                stream.CopyTo(memoryStream);
                var bytes = memoryStream.ToArray();
            }
        }

        [RelayCommand]
        public async Task SecureStorageCmd()
        {
            var result = await secureStorage.GetAsync("hello");
            service.Show(result ?? string.Empty, "Must be null");
            await secureStorage.SetAsync("hello", "world");
            result = await secureStorage.GetAsync("hello");
            service.Show(result ?? string.Empty, "Must be world");
            secureStorage.Remove("hello");
            result = await secureStorage.GetAsync("hello");
            service.Show(result ?? string.Empty, "Must be null");
        }

        [RelayCommand]
        public void VersionCmd()
        {
            versionTracking.Track();
            service.Show(versionTracking.CurrentVersion, "");
        }

        [RelayCommand]
        public void SemanticCmd()
        {
            semanticScreenReader.Announce("HEllo");
        }
    }
}
