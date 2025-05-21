//using ShowLogSDK;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace ShowLogLauncher
//{
//    public class SettingsProvider : ILogFileSettingsProvider
//    {
//        public string? LastLogFilePath
//        {
//            get => Properties.Settings.Default.LastLogFilePath;
//            set => Properties.Settings.Default.LastLogFilePath = value;
//        }

//        public void Save()
//        {
//            Properties.Settings.Default.Save();
//        }
//    }
//}
using ShowLogSDK;
using System;
using System.IO;
using System.Text.Json;

namespace ShowLogLauncher
{
    public class SettingsProvider : ILogFileSettingsProvider
    {
        private readonly string settingsFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
            "showLogs");

        private readonly string settingsFile;

        private SettingsData _settings;

        public SettingsProvider()
        {
            settingsFile = Path.Combine(settingsFolder, "settings.json");
            Load();
        }

        public string? LastLogFilePath
        {
            get => _settings.LastLogFilePath;
            set => _settings.LastLogFilePath = value;
        }

        public void Save()
        {
            Directory.CreateDirectory(settingsFolder);
            var json = JsonSerializer.Serialize(_settings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(settingsFile, json);
        }

        private void Load()
        {
            if (!Directory.Exists(settingsFolder))
                Directory.CreateDirectory(settingsFolder);

            if (File.Exists(settingsFile))
            {
                var json = File.ReadAllText(settingsFile);
                _settings = JsonSerializer.Deserialize<SettingsData>(json) ?? new SettingsData();
            }
            else
            {
                _settings = new SettingsData();
            }
        }

        private class SettingsData
        {
            public string? LastLogFilePath { get; set; }
        }
    }
}
