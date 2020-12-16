using Show_song_text.Database.Persistence;
using Show_song_text.Database.Repository;
using Show_song_text.Database.ViewModels;
using Show_song_text.Interfaces;
using Show_song_text.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Show_song_text.ViewModels
{
    public class AddSongToPlaylistViewModel : ViewModelBase
    {
        // PROPERTY SECTION START
        private ObservableCollection<SongViewModel> AllSongsCopy { get; set; } = new ObservableCollection<SongViewModel>();

        private String _searchBarText;
        public String SearchBarText
        {
            get
            {
                return _searchBarText;
            }
            set
            {
                _searchBarText = value;
                OnSearchBarTextChanged(value);
                OnPropertyChanged(nameof(SearchBarText));
            }
        }

        public ObservableCollection<SongViewModel> Songs { get; private set; }
           = new ObservableCollection<SongViewModel>();

        public ObservableCollection<SongViewModel> SelectedSongs { get; private set; }
            = new ObservableCollection<SongViewModel>();

        // PROPERTY END

        // VARIABLE SECTION START

        private readonly SongRepository songRepository;
        private readonly IPageService _pageService;

        // VARIABLE SEXTION END

        // COMMAND START
        public ICommand LoadSongsCommand { get; private set; }
        public ICommand SelectSongCommand { get; private set; }
        public ICommand UnselectSongCommand { get; private set; }
        public ICommand AddSongsToPlaylistCommand { get; private set; }
        //COMMAND END

        // CONSTRUCTOR START
        public AddSongToPlaylistViewModel()
        {
            songRepository = new SongRepository(DependencyService.Get<ISQLiteDb>());
            _pageService = new PageService();

            LoadSongsCommand = new Command(async () => await LoadSongs());
            SelectSongCommand = new Command<SongViewModel>(async song => await SelectSong(song));
            UnselectSongCommand = new Command<SongViewModel>(async song => await UnselectSong(song));
            AddSongsToPlaylistCommand = new Command(async () => await AddSongsToPlaylist());

            LoadSongsCommand.Execute(null);
        }
        // CONSTRUCTOR END

        // COMMAND METHOD START
        private async Task LoadSongs()
        {
            if (Songs != null && Songs.Count > 0)
            {
                Songs.Clear();
            }
            var songs = await songRepository.GetAllSongAsync();
            foreach (var song in songs)
            {
                song.IsCheckBoxVisible = true;
                Songs.Add(new SongViewModel(song));
            } 
            AllSongsCopy = Songs;
        }

        private async Task SelectSong(SongViewModel songViewModel)
        {
            SelectedSongs.Add(songViewModel);
        }
        private async Task UnselectSong(SongViewModel songViewModel)
        {
            SelectedSongs.Remove(songViewModel);
        }
        private async Task AddSongsToPlaylist()
        {
            MessagingCenter.Send(this, Events.AddSongsToPlaylist, SelectedSongs);
            await _pageService.PreviousDetailPage();
        }
        // COMMAND METHOD END


        // PROPERTY METHOD SECTION START
        public void OnSearchBarTextChanged(String text)
        {
            if (text == "")
                Songs = AllSongsCopy;
            var songs = AllSongsCopy.Where(s => s.Title.ToLower().Contains(text.ToLower()) || s.Artist.ToLower().Contains(text.ToLower()));
            Songs = new ObservableCollection<SongViewModel>(songs);
            OnPropertyChanged(nameof(Songs));
        }
        // PROPERTY METHOD SECTION STOP
    }

}
