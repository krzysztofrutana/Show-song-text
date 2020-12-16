using Show_song_text.Database.Models;
using Show_song_text.Database.Persistence;
using Show_song_text.Database.Repository;
using Show_song_text.Database.ViewModels;
using Show_song_text.Interfaces;
using Show_song_text.Utils;
using Show_song_text.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Show_song_text.ViewModels
{
    
    public class SongListViewModel : ViewModelBase
    {
        // PROPERTY SECTION START
        private ObservableCollection<SongViewModel> AllSongsCopy { get; set; } = new ObservableCollection<SongViewModel>();

        private String _searchBarText;
        public String SearchBarText { 
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
        
        private SongViewModel _selectedSong;

        public SongViewModel SelectedSong
        {
            get { return _selectedSong; }
            set { _selectedSong = value;
                OnSongSelected(value);
                OnPropertyChanged(nameof(SelectedSong)); }
        }

        private Boolean _showChooseOption;

        public Boolean ShowChooseOption
        {
            get { return _showChooseOption; }
            set { _showChooseOption = value;
                SetCheckBoxVisibility(value);
                OnPropertyChanged(nameof(ShowChooseOption));

            }
        }

        public ObservableCollection<SongViewModel> Songs { get; private set; }
            = new ObservableCollection<SongViewModel>();

        public ObservableCollection<SongViewModel> SelectedSongs { get; private set; }
            = new ObservableCollection<SongViewModel>();



        // PROPERTY SECTION END

        // VARIABLE SECTION START

        private readonly SongRepository songRepository;
        private readonly PlaylistRepository playlistRepository;
        private readonly IPageService _pageService;

        // VARIABLE SEXTION END
        
        // COMMAND SECTION START
        public ICommand LoadSongsCommand { get; private set; }
        public ICommand SelectSongCommand { get; private set; }
        public ICommand CreatePlaylistCommand { get; private set; }
        public ICommand AddToPlaylistCommand { get; private set; }
        public ICommand DeleteFromPlaylistCommand { get; private set; }

        // COMMAND SECTION END

        // CONSTRUCTOR
        public SongListViewModel()
        {
            songRepository = new SongRepository(DependencyService.Get<ISQLiteDb>());
            _pageService = new PageService();
            playlistRepository = new PlaylistRepository(DependencyService.Get<ISQLiteDb>());

            LoadSongsCommand = new Command(async () => await LoadSongs());
            SelectSongCommand = new Command<SongViewModel>(async song => await SelectSong(song));
            CreatePlaylistCommand = new Command(async () => await CreatePlaylist());
            AddToPlaylistCommand = new Command<SongViewModel>(async song => await AddToPlaylist(song));
            DeleteFromPlaylistCommand = new Command<SongViewModel>(async song => await DeleteFromPlaylist(song));

            MessagingCenter.Subscribe<SongAddAndDetailViewModel, Song>
                (this, Events.SongAdded, OnSongAdded);

            MessagingCenter.Subscribe<SongAddAndDetailViewModel, Song>
            (this, Events.SongUpdated, OnSongUpdated);

            MessagingCenter.Subscribe<SongAddAndDetailViewModel, Song>
            (this, Events.SongDeleted, OnSongDeleted);


            LoadSongsCommand.Execute(null);
            SetCheckBoxVisibility(false);

        }

        // CONSTRUCTION

        // COMMAND METHOD SECTIONS TART
        private async Task LoadSongs()
        {
            if(Songs != null && Songs.Count  > 0)
            {
                Songs.Clear();
            }
            var songs = await songRepository.GetAllSongAsync();
            foreach (var song in songs)
                Songs.Add(new SongViewModel(song));
            AllSongsCopy = Songs;
        }


        private async Task SelectSong(SongViewModel song)
        {
            if (song == null)
                return;
            await _pageService.ChangePageAsync(new SongAddAndDetailView());
            MessagingCenter.Send(this, Events.SendSong, song);
            SelectedSong = null;
        }

        private async Task CreatePlaylist()
        {
            List<Song> songsList = new List<Song>();
            foreach (SongViewModel songViewModel in SelectedSongs)
            {
                Song song = await songRepository.GetSong(songViewModel.Id);
                songsList.Add(song);
            }
            string playlistName = await _pageService.DisplayEntry("Wpisz nazwę playlisty", "");
            Playlist playlist = new Playlist()
            {
                Name = playlistName,
                Songs = songsList,
                CustomSongsOrder = false
            };
            await playlistRepository.AddPlaylist(playlist);
            var songtenp = await songRepository.GetSongWithChildren(songsList[0].Id);
            await _pageService.DisplayAlert("Sukces", "Playlista utworzona", "Ok");
            UnCheckAllSongs();
            SetCheckBoxVisibility(false);
            ShowChooseOption = false;
        }

        private async Task AddToPlaylist(SongViewModel songViewModel)
        {
            SelectedSongs.Add(songViewModel);
        }
        private async Task DeleteFromPlaylist(SongViewModel songViewModel)
        {
            SelectedSongs.Remove(songViewModel);
        }

        // COMMANDS METHODS SECTION END

        // MESSAGE CENTER METHOD SECTION START
        private void OnSongAdded(SongAddAndDetailViewModel source, Song song)
        {
            Songs.Add(new SongViewModel(song));
        }

        private void OnSongUpdated(SongAddAndDetailViewModel source, Song song)
        {
            var songInList = Songs.Single(s => s.Id == song.Id);

            songInList.Id = song.Id;
            songInList.Artist = song.Artist;
            songInList.Title = song.Title;
            songInList.Text = song.Text;
            songInList.Chords = song.Chords;
          
        }

        private void OnSongDeleted(SongAddAndDetailViewModel source, Song song)
        {
            Songs.Remove(Songs.Where(s => s.Id == song.Id).Single());
            OnPropertyChanged(nameof(Songs));

        }

        // MESSAGE CENTER METHOD SECTION END

        // PROPERTY METHOD SECTION START
        public void OnSearchBarTextChanged(String text)
        {
            if (text == "")
                Songs = AllSongsCopy;
            var songs = AllSongsCopy.Where(s => s.Title.ToLower().Contains(text.ToLower()) || s.Artist.ToLower().Contains(text.ToLower()));
            Songs = new ObservableCollection<SongViewModel>(songs);
            OnPropertyChanged(nameof(Songs));
        }

        private void SetCheckBoxVisibility(Boolean status)
        {
            foreach (SongViewModel song in Songs)
            {
                song.IsCheckBoxVisible = status;
            }
        }

        private void OnSongSelected(object page)
        {
            SelectSongCommand.Execute(page);
        }

        private void UnCheckAllSongs()
        {
            foreach(SongViewModel songViewModel in Songs)
            {
                if(songViewModel.IsChecked == true)
                {
                    songViewModel.IsChecked = false;
                }
            }
        }
        // PROPERTY METHOD SECTION END
    }
}
