using Show_song_text.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace Show_song_text.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        // PROPERTY SECTION START
        //private string fontSize = Settings.FontSize.ToString();
        public String FontSize { 
            get
            {
                return Settings.FontSize.ToString();
            }
            set
            {
                if (Settings.FontSize.Equals(value))
                    return;

                Settings.FontSize = value;
                OnPropertyChanged(nameof(FontSize));
            }
        }
        // PROPERTY SECTION END

        // COMMAND SECTION START
        public ICommand SaveSettingsCommand { get; private set; }
        // COMMAND SECTION END

        // CONSTRUCTOR START
        public SettingsViewModel()
        {
            SaveSettingsCommand = new Command(() => SaveSettings());
        }
        // CONSTRUCTION END

        // COMMAND METHOD START
        private void SaveSettings()
        {
            //Settings.FontSize = Double.Parse(FontSize);
        }
        // COMMAND METHOD END
    }
}
