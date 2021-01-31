using Show_song_text.Helpers;
using Show_song_text.Models;
using Show_song_text.Resources.Languages;
using Show_song_text.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Windows.Input;
using Xamarin.Forms;

namespace Show_song_text.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        #region Property
        public String FontSize
        {
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

        public ObservableCollection<LanguageModel> Languages { get; private set; }
            = new ObservableCollection<LanguageModel>();

        private LanguageModel selectedLanguage;

        public LanguageModel SelectedLanguage
        {
            get { return selectedLanguage; }
            set
            {
                selectedLanguage = value;
                string language = Thread.CurrentThread.CurrentUICulture.Name;
                if (value != null && !language.Contains(value.ShortName))
                {
                    SetCulture(value);
                }

                OnPropertyChanged(null);
            }
        }


        #endregion

        #region Commands

        #endregion

        #region Constructor
        public SettingsViewModel()
        {
            InitializeLanguageList();
            CheckLanguage();
        }
        #endregion

        #region Property methods
        private void InitializeLanguageList()
        {
            Languages.Add(new LanguageModel()
            {
                FullName = "English",
                ShortName = "en"
            });
            Languages.Add(new LanguageModel()
            {
                FullName = "Polski",
                ShortName = "pl"
            });
        }

        private void SetCulture(LanguageModel languageModel)
        {
            CultureInfo language = new CultureInfo(languageModel.ShortName);
            Thread.CurrentThread.CurrentUICulture = language;
            AppResources.Culture = language;
            Settings.ChoosenLanguage = languageModel.ShortName;
            Application.Current.MainPage = new MainPageView();
        }

        private void CheckLanguage()
        {
            
            if (!Settings.ChoosenLanguage.Equals("none"))
            {
                if (Settings.ChoosenLanguage.Equals("pl"))
                {
                    SelectedLanguage = Languages[1];
                }
                else
                {
                    SelectedLanguage = Languages[0];
                }
            }
            else
            {
                string language = Thread.CurrentThread.CurrentUICulture.Name;
                if (language.Contains("pl"))
                {
                    SelectedLanguage = Languages[1];
                }
                else
                {
                    SelectedLanguage = Languages[0];
                }
            }
        }
        #endregion
    }
}
