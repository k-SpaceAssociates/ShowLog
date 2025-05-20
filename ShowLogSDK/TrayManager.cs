using Hardcodet.Wpf.TaskbarNotification;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;



namespace ShowLogSDK
{
    public class TrayManager : IDisposable
    {
        private readonly TaskbarIcon _trayIcon;

        public TrayManager()
        {
            _trayIcon = new TaskbarIcon
            {
                Icon = SystemIcons.Application,
                ToolTipText = "Show Logs"
            };

            _trayIcon.TrayLeftMouseUp += TrayIcon_TrayLeftMouseUp;
        }

        private void TrayIcon_TrayLeftMouseUp(object? sender, RoutedEventArgs e)
        {
            try
            {
                string programData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                string logFilePath = Path.Combine(programData, "logs.txt");

                if (!File.Exists(logFilePath))
                {
                    MessageBox.Show($"Log file not found:\n{logFilePath}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                Process.Start(new ProcessStartInfo
                {
                    FileName = logFilePath,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to open log file:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void Dispose()
        {
            _trayIcon.Dispose();
        }
    }
}