using Hardcodet.Wpf.TaskbarNotification;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
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

            // Add context menu
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

                if (File.Exists(defaultLogPath))
                {
                    logFilePath = defaultLogPath;
                }
                else
                {
                    var result = MessageBox.Show($"Default log file not found:\n{defaultLogPath}\n\nDo you want to browse for a log file?",
                                                 "Log File Not Found",
                                                 MessageBoxButton.YesNo,
                                                 MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
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
                            return; // User cancelled the dialog
                        }
                    }
                    else
                    {
                        return; // User declined to browse
                    }
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