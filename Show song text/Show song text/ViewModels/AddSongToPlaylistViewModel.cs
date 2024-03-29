﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using ShowSongText.Abstraction;
using ShowSongText.Database.Abstraction;
using ShowSongText.Database.Repository;
using ShowSongText.Utils;
using ShowSongText.ViewModels.DTO;
using Xamarin.Forms;

namespace ShowSongText.ViewModels
{
    public class AddSongToPlaylistViewModel : ViewModelBase
    {
        #region Property
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

        #endregion


        #region Variable
        private readonly SongRepository songRepository;
        private readonly IPageService _pageService;
        #endregion

        #region Command
        public ICommand LoadSongsCommand { get; private set; }
        public ICommand SelectSongCommand { get; private set; }
        public ICommand UnselectSongCommand { get; private set; }
        public ICommand AddSongsToPlaylistCommand { get; private set; }
        #endregion

        #region Constructor
        public AddSongToPlaylistViewModel()
        {
            songRepository = new SongRepository(DependencyService.Get<ISQLiteDb>());
            _pageService = new PageService();

            LoadSongsCommand = new Command(async () => await LoadSongs());
            SelectSongCommand = new Command<SongViewModel>(song => SelectSong(song));
            UnselectSongCommand = new Command<SongViewModel>(song => UnselectSong(song));
            AddSongsToPlaylistCommand = new Command(async () => await AddSongsToPlaylist());

            LoadSongsCommand.Execute(null);
        }
        #endregion

        #region Command methods
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

        private void SelectSong(SongViewModel songViewModel)
        {
            SelectedSongs.Add(songViewModel);
        }
        private void UnselectSong(SongViewModel songViewModel)
        {
            SelectedSongs.Remove(songViewModel);
        }
        private async Task AddSongsToPlaylist()
        {
            MessagingCenter.Send(this, Events.AddSongsToPlaylist, SelectedSongs);
            await _pageService.PreviousDetailPage();
        }
        #endregion


        #region Property methods
        public void OnSearchBarTextChanged(String text)
        {
            if (text == "")
                Songs = AllSongsCopy;
            var songs = AllSongsCopy.Where(s => s.Title.ToLower().Contains(text.ToLower()) || s.Artist.ToLower().Contains(text.ToLower()));
            Songs = new ObservableCollection<SongViewModel>(songs);
            OnPropertyChanged(nameof(Songs));
        }
        #endregion
    }

}
