namespace Avae.Maui.Notifications;

#if ANDROID
public class MauiAppNotificationsActivity : MauiAppCompatActivity, Avalonia.Android.IActivityResultHandler
{
    Action<int, Android.App.Result, Android.Content.Intent?>? _activityResultAction;
    Action<int, string[], Android.Content.PM.Permission[]>? _permissionsResultAction;

    public Action<int, Android.App.Result, Android.Content.Intent?>? ActivityResult { get => _activityResultAction ??= OnActivityResult; set => _activityResultAction = value; }
    public Action<int, string[], Android.Content.PM.Permission[]>? RequestPermissionsResult { get => _permissionsResultAction ??= OnRequestPermissionsResult; set => _permissionsResultAction = value; }

    protected override void OnActivityResult(int requestCode, Android.App.Result resultCode, Android.Content.Intent? data)
    {
        base.OnActivityResult(requestCode, resultCode, data);
    }

    public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
    {
        base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
    }

    protected override void OnCreate(Android.OS.Bundle? savedInstanceState)
    {
        SystemNotificationService.Activity = this;

        base.OnCreate(savedInstanceState);
    }
}
#endif

