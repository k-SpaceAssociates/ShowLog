using Hardcodet.Wpf.TaskbarNotification;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;


namespace ShowLogSDK
{
    public interface ILogFileSettingsProvider
    {
        string? LastLogFilePath { get; set; }
        void Save();
    }
    public class TrayManager : IDisposable
    {
        private readonly TaskbarIcon _trayIcon;
        private readonly ILogFileSettingsProvider? _settingsProvider;

        public TrayManager(ILogFileSettingsProvider? settingsProvider = null)
        {
            _settingsProvider = settingsProvider;

            _trayIcon = new TaskbarIcon
            {
                Icon = SystemIcons.Application,
                ToolTipText = "Show Logs"
            };

            // Context menu setup
            var contextMenu = new ContextMenu();

            var openItem = new MenuItem { Header = "Open Log" };
            openItem.Click += TrayIcon_TrayLeftMouseUp;

            var exitItem = new MenuItem { Header = "Exit" };
            exitItem.Click += (s, e) => Application.Current.Shutdown();

            contextMenu.Items.Add(openItem);
            contextMenu.Items.Add(new Separator());
            contextMenu.Items.Add(exitItem);

            _trayIcon.ContextMenu = contextMenu;

            _trayIcon.TrayLeftMouseUp += TrayIcon_TrayLeftMouseUp;
        }
        private void TrayIcon_TrayLeftMouseUp(object? sender, RoutedEventArgs e)
        {
            try
            {
                var logFilePath = TryGetLogFilePath();
                if (logFilePath == null)
                    return;

                SaveLastLogFilePath(logFilePath);
                OpenLogFile(logFilePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to open log file:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                // Optionally log full exception here
            }
        }

        private string? TryGetLogFilePath()
        {
            var programData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            var defaultLogPath = Path.Combine(programData, "logs.txt");

            if (File.Exists(defaultLogPath))
                return defaultLogPath;

            var lastUsedPath = _settingsProvider?.LastLogFilePath;
            if (!string.IsNullOrWhiteSpace(lastUsedPath) && File.Exists(lastUsedPath))
            {
                var openLast = MessageBox.Show(
                    $"Default log file not found:\n{defaultLogPath}\n\nPreviously used file found:\n{lastUsedPath}\n\nOpen it instead?",
                    "Log File Not Found",
                    MessageBoxButton.YesNoCancel,
                    MessageBoxImage.Question);

                if (openLast == MessageBoxResult.Yes)
                    return lastUsedPath;

                if (openLast == MessageBoxResult.Cancel)
                    return null;
                // else fall through to browse
            }

            var browse = MessageBox.Show(
                $"Default log file not found:\n{defaultLogPath}\n\nDo you want to browse for a log file?",
                "Log File Not Found",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (browse != MessageBoxResult.Yes)
                return null;

            var openFileDialog = new OpenFileDialog
            {
                Title = "Select Log File",
                Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*",
                InitialDirectory = programData
            };

            return openFileDialog.ShowDialog() == true ? openFileDialog.FileName : null;
        }

        private void SaveLastLogFilePath(string path)
        {
            if (_settingsProvider == null)
                return;

            _settingsProvider.LastLogFilePath = path;
            _settingsProvider.Save();
        }

        private void OpenLogFile(string path)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = path,
                UseShellExecute = true
            });
        }
        public void Dispose()
        {
            _trayIcon.Dispose();
        }
    }
}