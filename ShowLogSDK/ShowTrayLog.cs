

using System;
using System.Diagnostics;
using System.Drawing;
using Hardcodet.Wpf.TaskbarNotification;
using System;
using System.IO;
using System.Windows;

namespace ShowLogSDK
{
    public static class ShowLogTray
    {
        private static TrayManager? _trayManager;

        public static void Start()
        {
            if (_trayManager == null)
                _trayManager = new TrayManager();
        }

        public static void Stop()
        {
            _trayManager?.Dispose();
            _trayManager = null;
        }
    }
}
