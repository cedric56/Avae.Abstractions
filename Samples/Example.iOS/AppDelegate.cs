using Avalonia;
using Avalonia.iOS;
using Foundation;

namespace Example.iOS;

// The UIApplicationDelegate for the application. This class is responsible for launching the 
// User Interface of the application, as well as listening (and optionally responding) to 
// application events from iOS.
[Register("AppDelegate")]
public partial class AppDelegate : AvaloniaAppDelegate<Avalonia.Application>
{
    protected override AppBuilder CreateAppBuilder()
    {
        return App.CreateApp()
            .UseiOS();
    }
}
