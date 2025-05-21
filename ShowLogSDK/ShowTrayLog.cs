

using Hardcodet.Wpf.TaskbarNotification;
using System;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows;

namespace ShowLogSDK
{
    public static class ShowLogTray
    {
        private static TrayManager? _trayManager;

        public static void Start(ILogFileSettingsProvider settingsProvider)
        {
            if (_trayManager == null)
                _trayManager = new TrayManager(settingsProvider);
        }

        public static void Stop()
        {
            _trayManager?.Dispose();
            _trayManager = null;
        }
    }
}
