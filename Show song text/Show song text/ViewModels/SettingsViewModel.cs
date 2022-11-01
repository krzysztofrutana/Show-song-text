using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Newtonsoft.Json;
using ShowSongText.Abstraction;
using ShowSongText.Database.Abstraction;
using ShowSongText.Database.Repository;
using ShowSongText.Helpers;
using ShowSongText.Models;
using ShowSongText.Models.DatabaseBackup;
using ShowSongText.Resources.Languages;
using ShowSongText.Utils;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace ShowSongText.ViewModels
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
        public bool ShowChords
        {
            get { return Settings.ShowChords; }
            set
            {
                if (Settings.ShowChords == value)
                    return;

                Settings.ShowChords = value;
                OnPropertyChanged(nameof(ShowChords));
            }
        }


        #endregion

        #region Variables
        private readonly PlaylistRepository playlistRepository;
        private readonly PositionRepository positionRepository;
        private readonly SongPlaylistRepository songPlaylistRepository;
        private readonly SongPositionRepository songPositionRepository;
        private readonly SongRepository songRepository;
        private readonly IPageService _pageService;
        #endregion

        #region Commands

        public ICommand CreateBackupCommand { get; private set; }
        public ICommand RestoreBackupCommand { get; private set; }
        #endregion

        #region Constructor
        public SettingsViewModel()
        {
            playlistRepository = new PlaylistRepository(DependencyService.Get<ISQLiteDb>());
            positionRepository = new PositionRepository(DependencyService.Get<ISQLiteDb>());
            songPlaylistRepository = new SongPlaylistRepository(DependencyService.Get<ISQLiteDb>());
            songPositionRepository = new SongPositionRepository(DependencyService.Get<ISQLiteDb>());
            songRepository = new SongRepository(DependencyService.Get<ISQLiteDb>());
            _pageService = new PageService();

            CreateBackupCommand = new Command(async () => await CreateBackup());
            RestoreBackupCommand = new Command(async () => await RestoreBackup());

            InitializeLanguageList();
            CheckLanguage();
        }
        #endregion

        #region Commands methods
        // Write file with current date to json file in external download folder
        private async Task CreateBackup()
        {
            string filename = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss") + ".json";

            DatabaseModel dm = new DatabaseModel();

            dm.Playlists = await playlistRepository.GetAllPlaylistArrayAsync();
            dm.Positions = await positionRepository.GetAllPositionArrayAsync();
            dm.SongPlaylists = await songPlaylistRepository.GetAllArrayAsync();
            dm.SongPositions = await songPositionRepository.GetAllArrayAsync();
            dm.Songs = await songRepository.GetAllSongArrayAsync();

            var json = JsonConvert.SerializeObject(dm);

            DependencyService.Get<IWirteService>().WirteFile(filename, json);
        }

        private async Task RestoreBackup()
        {
            try
            {

                var options = new PickOptions
                {
                    PickerTitle = "Please select a json file",
                };

                var result = await FilePicker.PickAsync(options);
                if (result != null)
                {
                    Stream stream = await result.OpenReadAsync();
                    StreamReader streamReader = new StreamReader(stream);
                    string json = streamReader.ReadToEnd();
                    var db = JsonConvert.DeserializeObject<DatabaseModel>(json);

                    foreach (var item in db.Playlists)
                    {
                        playlistRepository.AddPlaylist(item);
                    }
                    foreach (var item in db.Positions)
                    {
                        positionRepository.AddPosition(item);
                    }
                    foreach (var item in db.SongPlaylists)
                    {
                        songPlaylistRepository.AddSongPlaylistRelation(item);
                    }
                    foreach (var item in db.SongPositions)
                    {
                        songPositionRepository.AddSongPositionRelation(item);
                    }
                    foreach (var item in db.Songs)
                    {
                        songRepository.AddSong(item);
                    }
                }
            }
            catch (Exception ex)
            {
            }



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
            Application.Current.MainPage = new FlyoutPage();
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
