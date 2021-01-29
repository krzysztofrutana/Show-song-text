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
        #region Property
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
        #endregion

        #region Commands

        #endregion

        #region Constructor
        public SettingsViewModel()
        {
          
        }
        #endregion
    }
}
