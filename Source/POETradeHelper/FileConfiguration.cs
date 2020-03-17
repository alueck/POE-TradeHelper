using System;
using System.IO;

namespace POETradeHelper
{
    public static class FileConfiguration
    {
        public static string PoeTradeHelperAppDataFolder
        {
            get
            {
                string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                return Path.Combine(appDataFolder, "POETradeHelper");
            }
        }

        public static string PoeTradeHelperAppSettingsPath => Path.Combine(PoeTradeHelperAppDataFolder, "appsettings.json");
    }
}