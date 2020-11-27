using Show_song_text.Database.DOA;
using Show_song_text.Database.Models;
using Show_song_text.Database.ViewModels;
using Show_song_text.Interfaces;
using Show_song_text.Utils;
using Show_song_text.Views;
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
    
    public class SongListViewModel : ViewModelBase
    {
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
        public void OnSearchBarTextChanged(String text)
        {
            if (text == "")
                Songs = AllSongsCopy;
            var songs = AllSongsCopy.Where(s => s.Title.ToLower().Contains(text.ToLower()) || s.Artist.ToLower().Contains(text.ToLower()));
            Songs = new ObservableCollection<SongViewModel>(songs);
        }
        private SongViewModel _selectedSong;

        public SongViewModel SelectedSong
        {
            get { return _selectedSong; }
            set { _selectedSong = value;
                OnPropertyChanged(nameof(SelectedSong)); }
        }

        private readonly ISongDAO _songDAO;
        private readonly IPageService _pageService;

        public ObservableCollection<SongViewModel> Songs { get; private set; }
            = new ObservableCollection<SongViewModel>();

        public ICommand LoadSongsCommand { get; private set; }
        public ICommand AddSongCommand { get; private set; }
        public ICommand SelectSongCommand { get; private set; }
        public ICommand DeleteSongCommand { get; private set; }


        public SongListViewModel(ISongDAO songDAO, IPageService pageService)
        {
            _songDAO = songDAO;
            _pageService = pageService;

            LoadSongsCommand = new Command(async () => await LoadSongs());
            AddSongCommand = new Command(async () => await AddSong());
            SelectSongCommand = new Command<SongViewModel>(async song => await SelectSong(song));
            DeleteSongCommand = new Command<SongViewModel>(async song => await DeleteSong(song));

            MessagingCenter.Subscribe<SongDetailViewModel, Song>
                (this, Events.SongAdded, OnSongAdded);

            MessagingCenter.Subscribe<SongDetailViewModel, Song>
            (this, Events.SongUpdated, OnSongUpdated);

            LoadSongsCommand.Execute(null);
        }

        
        private async Task LoadSongs()
        {
            if(Songs != null && Songs.Count  > 0)
            {
                Songs.Clear();
            }
            var songs = await _songDAO.GetAllSongAsync();
            foreach (var song in songs)
                Songs.Add(new SongViewModel(song));
            AllSongsCopy = Songs;
        }

        private async Task AddSong()
        {
            await _pageService.PushAsync(new SongDetailView(new SongViewModel()));
        }

        private async Task SelectSong(SongViewModel song)
        {
            if (song == null)
                return;

            SelectedSong = null;
            await _pageService.PushAsync(new SongDetailView(song));
        }

        private async Task DeleteSong(SongViewModel song)
        {
            if (await _pageService.DisplayAlert("Ostrzeżenie", $"Jesteś pewny, że chcesz usunąć {song.FullName}?", "Tak", "Nie"))
            {
                Songs.Remove(song);

                var songToDelete = await _songDAO.GetSong(song.Id);
                await _songDAO.DeleteSong(songToDelete);
            }
        }

        private void OnSongAdded(SongDetailViewModel source, Song song)
        {
            Songs.Add(new SongViewModel(song));
        }

        private void OnSongUpdated(SongDetailViewModel source, Song song)
        {
            var songInList = Songs.Single(s => s.Id == song.Id);

            songInList.Id = song.Id;
            songInList.Artist = song.Artist;
            songInList.Title = song.Title;
            songInList.Text = song.Text;
            songInList.Chords = song.Chords;
          
        }


    }
}
