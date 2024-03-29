﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using ShowSongText.Abstraction;
using ShowSongText.Database.Abstraction;
using ShowSongText.Database.Models;
using ShowSongText.Database.Repository;
using ShowSongText.Resources.Languages;
using ShowSongText.Utils;
using ShowSongText.ViewModels.DTO;
using ShowSongText.Views;
using Xamarin.Forms;

namespace ShowSongText.ViewModels
{

    public class SongListViewModel : ViewModelBase
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

        private SongViewModel _selectedSong;

        public SongViewModel SelectedSong
        {
            get { return _selectedSong; }
            set
            {
                _selectedSong = value;
                OnSongSelected(value);
                OnPropertyChanged(nameof(SelectedSong));
            }
        }

        private Boolean _showChooseOption;

        public Boolean ShowChooseOption
        {
            get { return _showChooseOption; }
            set
            {
                _showChooseOption = value;
                SetCheckBoxVisibility(value);
                OnPropertyChanged(nameof(ShowChooseOption));

            }
        }

        public ObservableCollection<SongViewModel> Songs { get; private set; }
            = new ObservableCollection<SongViewModel>();

        public ObservableCollection<SongViewModel> SelectedSongs { get; private set; }
            = new ObservableCollection<SongViewModel>();
        #endregion

        #region Variables
        private readonly SongRepository songRepository;
        private readonly PlaylistRepository playlistRepository;
        private readonly IPageService _pageService;
        #endregion

        #region Commands
        public ICommand LoadSongsCommand { get; private set; }
        public ICommand SelectSongCommand { get; private set; }
        public ICommand CreatePlaylistCommand { get; private set; }
        public ICommand AddToPlaylistCommand { get; private set; }
        public ICommand DeleteFromPlaylistCommand { get; private set; }
        public ICommand StartSongPresentationCommand { get; private set; }
        #endregion

        #region Constructor
        public SongListViewModel()
        {
            songRepository = new SongRepository(DependencyService.Get<ISQLiteDb>());
            _pageService = new PageService();
            playlistRepository = new PlaylistRepository(DependencyService.Get<ISQLiteDb>());

            LoadSongsCommand = new Command(async () => await LoadSongs());
            SelectSongCommand = new Command<SongViewModel>(async song => await SelectSong(song));
            CreatePlaylistCommand = new Command(async () => await CreatePlaylist());
            AddToPlaylistCommand = new Command<SongViewModel>(song => AddToPlaylist(song));
            DeleteFromPlaylistCommand = new Command<SongViewModel>(song => DeleteFromPlaylist(song));
            StartSongPresentationCommand = new Command<int>(async id => await StartPresentation(id));

            MessagingCenter.Subscribe<SongAddAndDetailViewModel, Song>
                (this, Events.SongAdded, OnSongAdded);

            MessagingCenter.Subscribe<SongAddAndDetailViewModel, Song>
            (this, Events.SongUpdated, OnSongUpdated);

            MessagingCenter.Subscribe<SongAddAndDetailViewModel, Song>
            (this, Events.SongDeleted, OnSongDeleted);


            LoadSongsCommand.Execute(null);
            SetCheckBoxVisibility(false);

        }
        #endregion

        #region Commands methods
        private async Task LoadSongs()
        {
            if (Songs != null && Songs.Count > 0)
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
            string playlistName = await _pageService.DisplayEntry(AppResources.DisplayEntry_EnterPlaylistName, "");
            if (String.IsNullOrEmpty(playlistName))
            {
                await _pageService.DisplayAlert(AppResources.AlertDialog_Error, AppResources.AlertDialog_NameCannotBeEmpty, AppResources.AlertDialog_OK);
            }
            else
            {
                Playlist playlist = new Playlist()
                {
                    Name = playlistName,
                    Songs = songsList,
                    CustomSongsOrder = false
                };
                await playlistRepository.AddPlaylist(playlist);
                await _pageService.DisplayAlert(AppResources.AlertDialog_Success, AppResources.AlertDialog_PlaylistCreated, AppResources.AlertDialog_OK);
                UnCheckAllSongs();
                SetCheckBoxVisibility(false);
                ShowChooseOption = false;
            }
        }

        private void AddToPlaylist(SongViewModel songViewModel)
        {
            SelectedSongs.Add(songViewModel);
        }
        private void DeleteFromPlaylist(SongViewModel songViewModel)
        {
            SelectedSongs.Remove(songViewModel);
        }

        private async Task StartPresentation(int id)
        {
            SongViewModel song = Songs.Where(s => s.Id == id).FirstOrDefault();
            await _pageService.ChangePageAsync(new SongTextPresentationView());
            MessagingCenter.Send(this, Events.SendSongToPresentation, song);
        }
        #endregion

        #region Message center methods
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
            foreach (SongViewModel songViewModel in Songs)
            {
                if (songViewModel.IsChecked == true)
                {
                    songViewModel.IsChecked = false;
                }
            }
        }
        #endregion
    }
}
