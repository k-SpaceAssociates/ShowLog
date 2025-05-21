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
                string programData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                string defaultLogPath = Path.Combine(programData, "logs.txt");
                string? logFilePath = null;

                // Use last saved path from injected settings provider if available
                string? lastUsedPath = _settingsProvider?.LastLogFilePath;

                if (File.Exists(defaultLogPath))
                {
                    logFilePath = defaultLogPath;
                }
                else if (!string.IsNullOrWhiteSpace(lastUsedPath) && File.Exists(lastUsedPath))
                {
                    var result = MessageBox.Show(
                        $"Default log file not found:\n{defaultLogPath}\n\nPreviously used file found:\n{lastUsedPath}\n\nOpen it instead?",
                        "Log File Not Found",
                        MessageBoxButton.YesNoCancel,
                        MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        logFilePath = lastUsedPath;
                    }
                    else if (result == MessageBoxResult.Cancel)
                    {
                        return;
                    }
                    // else fall through to browse
                }

                if (logFilePath == null)
                {
                    var result = MessageBox.Show(
                        $"Default log file not found:\n{defaultLogPath}\n\nDo you want to browse for a log file?",
                        "Log File Not Found",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (result != MessageBoxResult.Yes)
                        return;

                    OpenFileDialog openFileDialog = new OpenFileDialog
                    {
                        Title = "Select Log File",
                        Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*",
                        InitialDirectory = programData
                    };

                    if (openFileDialog.ShowDialog() == true)
                    {
                        logFilePath = openFileDialog.FileName;
                    }
                    else
                    {
                        return; // user cancelled browse
                    }
                }

                // Save the chosen path for next time
                if (_settingsProvider != null)
                {
                    _settingsProvider.LastLogFilePath = logFilePath;
                    _settingsProvider.Save();

                    Process.Start(new ProcessStartInfo
                    {
                        FileName = logFilePath,
                        UseShellExecute = true
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to open log file:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        //private string? GetLogFilePath()
        //{
        //    string programData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
        //    string defaultLogPath = Path.Combine(programData, "logs.txt");

        //    if (File.Exists(defaultLogPath))
        //        return defaultLogPath;

        //    var result = MessageBox.Show(
        //        $"Default log file not found:\n{defaultLogPath}\n\nDo you want to browse for a log file?",
        //        "Log File Not Found",
        //        MessageBoxButton.YesNo,
        //        MessageBoxImage.Question);

        //    if (result != MessageBoxResult.Yes)
        //        return null;

        //    OpenFileDialog openFileDialog = new OpenFileDialog
        //    {
        //        Title = "Select Log File",
        //        Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*",
        //        InitialDirectory = programData
        //    };

        //    return openFileDialog.ShowDialog() == true ? openFileDialog.FileName : null;
        //}
        public void Dispose()
        {
            _trayIcon.Dispose();
        }
    }
}