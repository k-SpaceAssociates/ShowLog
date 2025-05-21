using ShowLogSDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowLogLauncher
{
    public class SettingsProvider : ILogFileSettingsProvider
    {
        public string? LastLogFilePath
        {
            get => Properties.Settings.Default.LastLogFilePath;
            set => Properties.Settings.Default.LastLogFilePath = value;
        }

        public void Save()
        {
            Properties.Settings.Default.Save();
        }
    }
}
