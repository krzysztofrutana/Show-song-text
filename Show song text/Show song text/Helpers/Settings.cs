using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace Show_song_text.Helpers
{
    /// <summary>
    /// This is the Settings static class that can be used in your Core solution or in any
    /// of your client applications. All settings are laid out the same exact way with getters
    /// and setters. 
    /// </summary>
    public static class Settings
    {
        private static ISettings AppSettings
        {
            get
            {
                if (CrossSettings.IsSupported)
                    return CrossSettings.Current;

                return null; // or your custom implementation 
            }
        }

        #region Setting Constants

        private const string SettingsKey = "settings_key";
        private static readonly string SettingsDefault = string.Empty;
        private static readonly string FontSizeDefault = "20";
        private static readonly bool ClientIsConnectedDefault = false;
        private static readonly bool ServerIsRunningDefault = false;

        #endregion


        public static string GeneralSettings
        {
            get
            {
                return AppSettings.GetValueOrDefault(SettingsKey, SettingsDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(SettingsKey, value);
            }
        }

        public static string FontSize
        {
            get
            {
                return AppSettings.GetValueOrDefault(nameof(FontSize), FontSizeDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(nameof(FontSize), value);
            }
        }

        public static bool ClientIsConnected
        {
            get
            {
                return AppSettings.GetValueOrDefault(nameof(ClientIsConnected), ClientIsConnectedDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(nameof(ClientIsConnected), value);
            }
        }

        public static string ConnectedServerIP
        {
            get
            {
                return AppSettings.GetValueOrDefault(nameof(ConnectedServerIP), string.Empty);
            }
            set
            {
                AppSettings.AddOrUpdateValue(nameof(ConnectedServerIP), value);
            }
        }

        public static int ConnectedServerPort
        {
            get
            {
                return AppSettings.GetValueOrDefault(nameof(ConnectedServerPort), 11000);
            }
            set
            {
                AppSettings.AddOrUpdateValue(nameof(ConnectedServerPort), value);
            }
        }


        public static bool ServerIsRunning
        {
            get
            {
                return AppSettings.GetValueOrDefault(nameof(ServerIsRunning), ServerIsRunningDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(nameof(ServerIsRunning), value);
            }
        }

        public static int ServerPort
        {
            get
            {
                return AppSettings.GetValueOrDefault(nameof(ServerPort), 11000);
            }
            set
            {
                AppSettings.AddOrUpdateValue(nameof(ServerPort), value);
            }
        }

        public static int ServerClientConnectedQty
        {
            get
            {
                return AppSettings.GetValueOrDefault(nameof(ServerClientConnectedQty), 0);
            }
            set
            {
                AppSettings.AddOrUpdateValue(nameof(ServerClientConnectedQty), value);
            }
        }


    }
}