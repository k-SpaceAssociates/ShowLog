using System.Configuration;
using System.Data;
using System.Windows;
using System.Threading;
using ShowLogSDK;

namespace ShowLogLaucher
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            ShowLogTray.Start();
            // Optionally hide the main window if you want
            ShutdownMode = ShutdownMode.OnExplicitShutdown;
        }

        protected override void OnExit(ExitEventArgs e)
        {
            ShowLogTray.Stop();
            base.OnExit(e);
        }
    }

}
