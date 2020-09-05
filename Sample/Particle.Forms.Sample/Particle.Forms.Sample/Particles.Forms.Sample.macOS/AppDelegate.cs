using AppKit;
using Foundation;
using Particle.Forms.Sample;
using Xamarin.Forms.Platform.MacOS;

namespace Particles.Forms.Sample.macOS
{
    [Register("AppDelegate")]
    public class AppDelegate : FormsApplicationDelegate
    {
        NSWindow _window;

        public AppDelegate()
        {
            var style = NSWindowStyle.Closable | NSWindowStyle.Resizable | NSWindowStyle.Titled;

            var rect = new CoreGraphics.CGRect(200, 1000, 1024, 768);
            _window = new NSWindow(rect, style, NSBackingStore.Buffered, false)
            {
                Title = "Xamarin.Forms on Mac!",
                TitleVisibility = NSWindowTitleVisibility.Hidden
            };
            // choose your own Title here
        }

        public override NSWindow MainWindow => _window;

        public override void DidFinishLaunching(NSNotification notification)
        {
            // Insert code here to initialize your application

            Xamarin.Forms.Forms.Init();
            // Xamarin.Forms.FormsMaterial.Init();
            LoadApplication(new App());
            base.DidFinishLaunching(notification);
        }

        public override void WillTerminate(NSNotification notification)
        {
            // Insert code here to tear down your application
        }
    }
}