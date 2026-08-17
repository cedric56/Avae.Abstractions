using Android.App;
using Android.Runtime;

namespace Example.Maui
{
    [Application]
    public class MainApplication : MauiApplication
    {
        public MainApplication(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
        }

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();


        public override void OnCreate()
        {
            base.OnCreate();
            Android.Util.Log.Info("MYAPP", "OnCreate called");

            // ✅ Global exception handler
            AndroidEnvironment.UnhandledExceptionRaiser += (sender, args) =>
            {
                Android.Util.Log.Error("MYAPP", $"❌ Unhandled Android Exception: {args.Exception}");
            };
        }
    }
}
