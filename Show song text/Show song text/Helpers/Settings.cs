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

    }
}