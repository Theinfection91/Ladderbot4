using Ladderbot4.Data;
using Ladderbot4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ladderbot4.Managers
{
    public class SettingsManager
    {
        private readonly SettingsData _settingsData;
        public Settings Settings { get; set; }

        public SettingsManager(SettingsData settingsData)
        {
            _settingsData = settingsData;
            Settings = _settingsData.LoadSettings();
            Console.WriteLine("Test");
        }

        public void LoadSettingsData()
        {
            Settings = _settingsData.LoadSettings();
        }
    }
}
