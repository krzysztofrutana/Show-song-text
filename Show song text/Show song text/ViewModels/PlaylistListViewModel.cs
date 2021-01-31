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
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Show_song_text.ViewModels
{
    public class PlaylistListViewModel : ViewModelBase
    {
        #region Property
        public ObservableCollection<PlaylistViewModel> Playlists { get; private set; }
            = new ObservableCollection<PlaylistViewModel>();

        private PlaylistViewModel _selectedPlaylist;

        public PlaylistViewModel SelectedPlaylist
        {
            get { return _selectedPlaylist; }
            set
            {
                _selectedPlaylist = value;
                OnPlaylistSelected(value);
                OnPropertyChanged(nameof(SelectedPlaylist));
            }
        }

        #endregion

        #region Variables
        private readonly PlaylistRepository playlistRepository;
        private readonly IPageService _pageService;
        #endregion

        #region Commands
        public ICommand LoadPlaylistsCommand { get; private set; }
        public ICommand SelectPlaylistCommand { get; private set; }
        #endregion

        #region Constructor
        public PlaylistListViewModel()
        {
            playlistRepository = new PlaylistRepository(DependencyService.Get<ISQLiteDb>());
            _pageService = new PageService();

            LoadPlaylistsCommand = new Command(async () => await LoadPlaylists());
            SelectPlaylistCommand = new Command<PlaylistViewModel>(async playlist => await SelectPlaylist(playlist));

            MessagingCenter.Subscribe<PlaylistDetailViewModel, Playlist>
            (this, Events.PlaylistUpdated, OnPlaylistUpdated);

            MessagingCenter.Subscribe<PlaylistDetailViewModel, Playlist>
            (this, Events.PlaylistDeleted, OnPlaylistDeleted);

            LoadPlaylistsCommand.Execute(null);
        }
        #endregion

        #region Commands methods
        private async Task LoadPlaylists()
        {
            if (Playlists != null && Playlists.Count > 0)
            {
                Playlists.Clear();
            }
            var playlists = await playlistRepository.GetAllPlaylistAsync();
            foreach (var playlist in playlists)
                Playlists.Add(new PlaylistViewModel(playlist));
        }
        private async Task SelectPlaylist(PlaylistViewModel playlist)
        {
            if (playlist == null)
                return;
            await _pageService.ChangePageAsync(new PlaylistDetailView());
            MessagingCenter.Send(this, Events.SendPlaylist, playlist);
            SelectedPlaylist = null;
        }
        #endregion

        #region Message center methods
        private void OnPlaylistUpdated(PlaylistDetailViewModel source, Playlist playlist)
        {
            var playlistInList = Playlists.Single(p => p.Id == playlist.Id);

            playlistInList.Id = playlist.Id;
            playlistInList.Name = playlist.Name;
            playlistInList.Songs = playlist.Songs;

        }

        private void OnPlaylistDeleted(PlaylistDetailViewModel source, Playlist playlist)
        {
            Playlists.Remove(Playlists.Where(p => p.Id == playlist.Id).Single());
            OnPropertyChanged(nameof(Playlists));

        }
        #endregion

        #region Property methods
        private void OnPlaylistSelected(object page)
        {
            SelectPlaylistCommand.Execute(page);
        }
        #endregion

    }
}
