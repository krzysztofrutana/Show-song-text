using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace ShowSongText.Helpers
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
        private static readonly string DefaultLanguage = "none";
        private static readonly string DefaultGeniusClientID = "i56qNUJopP-mVbw8KpN002UsYMjsxCFFXxIOp1YXc43tTIPIdB82D-Gjj3aznjij";
        private static readonly string DefaultGeniusSecretKey = "UPwwGF9XLFqYUE5JuWKc3MLzOwlocWCbQOrThv8F9Jtx3f6zgnpL9afYqKSBFAyI6J6EPmorpLGTOPkUAvEuwg";
        private static readonly string DefaultGeniusToken = "3ng8PEukcc1t31mEzHBFuSP-Njxp-Rb7_rybYRZDgPF-iu0z4pBr9wlzNIYCHwaP";
        private static readonly bool DefaultShowChords = true;

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

        public static string ChoosenLanguage
        {
            get
            {
                return AppSettings.GetValueOrDefault(nameof(ChoosenLanguage), DefaultLanguage);
            }
            set
            {
                AppSettings.AddOrUpdateValue(nameof(ChoosenLanguage), value);
            }
        }

        public static string GeniusClientID
        {
            get
            {
                return AppSettings.GetValueOrDefault(nameof(GeniusClientID), DefaultGeniusClientID);
            }
            set
            {
                AppSettings.AddOrUpdateValue(nameof(GeniusClientID), value);
            }
        }

        public static string GeniusSecretKey
        {
            get
            {
                return AppSettings.GetValueOrDefault(nameof(GeniusSecretKey), DefaultGeniusSecretKey);
            }
            set
            {
                AppSettings.AddOrUpdateValue(nameof(GeniusSecretKey), value);
            }
        }

        public static string GeniusToken
        {
            get
            {
                return AppSettings.GetValueOrDefault(nameof(GeniusToken), DefaultGeniusToken);
            }
            set
            {
                AppSettings.AddOrUpdateValue(nameof(GeniusToken), value);
            }
        }

        public static bool ShowChords
        {
            get
            {
                return AppSettings.GetValueOrDefault(nameof(ShowChords), DefaultShowChords);
            }
            set
            {
                AppSettings.AddOrUpdateValue(nameof(ShowChords), value);
            }
        }
    }
}