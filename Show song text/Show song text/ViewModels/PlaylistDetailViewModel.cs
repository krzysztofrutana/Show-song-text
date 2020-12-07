using Show_song_text.Database.Models;
using Show_song_text.Database.Persistence;
using Show_song_text.Database.Repository;
using Show_song_text.Database.ViewModels;
using Show_song_text.Interfaces;
using Show_song_text.Utils;
using Show_song_text.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Show_song_text.ViewModels
{
    public class PlaylistDetailViewModel : ViewModelBase
    {
        // PROPERTY START
        public Playlist Playlist { get; private set; }
        private string _Name;
        public string Name
        {
            get
            {
                return _Name;

            }
            set
            {
                _Name = value;
                Playlist.Name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        private List<Song> _SongsList;
        public List<Song> SongsList
        {
            get
            {
                return _SongsList;

            }
            set
            {
                _SongsList = value;
                Playlist.Songs = value;
                OnPropertyChanged(nameof(SongsList));
            }
        }
        //PROPERTIES SECTION END

        // VARIABLE SECTION START
        private readonly PlaylistRepository playlistRepository;
        private readonly IPageService _pageService;
        // VARIABLE SEXTION END

        // COMMAND START
        public ICommand SaveCommand { get; private set; }
        public ICommand DeletePlaylistCommand { get; private set; }
        public ICommand StartPresentationCommand { get; private set; }
        // COMMAND STOP

        // CONSTRUCTOR START
        public PlaylistDetailViewModel()
        {
            playlistRepository = new PlaylistRepository(DependencyService.Get<ISQLiteDb>());
            _pageService = new PageService();

            SaveCommand = new Command(async () => await Save());
            DeletePlaylistCommand = new Command(async () => await DeletePlaylist());
            StartPresentationCommand = new Command(async () => await StartPresentation());

            MessagingCenter.Subscribe<PlaylistListViewModel, PlaylistViewModel>
            (this, Events.SendPlaylist, OnPlaylistSended);

        }
        // CONSTRUCTOR END

        // COMMAND METHOD START
        private async Task DeletePlaylist()
        {
            if (await _pageService.DisplayAlert("Ostrzeżenie", $"Jesteś pewny, że chcesz usunąć {Playlist.Name}?", "Tak", "Nie"))
            {

                var playlistToDelete = await playlistRepository.GetPlaylist(Playlist.Id);
                await playlistRepository.DeletePlaylist(playlistToDelete);
                MessagingCenter.Send(this, Events.PlaylistDeleted, Playlist);
                _pageService.ChangePage(new PlaylistListView());
            }
        }

        async Task Save()
        {
            if (Playlist.Id != 0)
            {
                if (String.IsNullOrWhiteSpace(Playlist.Name))
                {
                    await _pageService.DisplayAlert("Bład", "Wprowadź nazwę.", "OK");
                    return;
                }
                else
                {
                    await playlistRepository.UpdatePlaylist(Playlist);
                    MessagingCenter.Send(this, Events.PlaylistUpdated, Playlist);
                }
            }
            await _pageService.PreviousDetailPage();
        }

        async Task StartPresentation()
        {
            await _pageService.ChangePageAsync(new SongTextPresentationView());
            MessagingCenter.Send(this, Events.SendPlaylistToPresentation, new PlaylistViewModel(Playlist));
        }
        // MESSAGE CENTER START
        private async void OnPlaylistSended(PlaylistListViewModel source, PlaylistViewModel playlistViewModel)
        {
            
            if (playlistViewModel != null)
            {
                //IsFloatingButtonVisible = true;
                Playlist = await playlistRepository.GetPlaylistWithSongs(playlistViewModel.Id);
                Name = Playlist.Name;
                SongsList = Playlist.Songs;
            }
            MessagingCenter.Unsubscribe<PlaylistListViewModel, PlaylistViewModel>(this, Events.SendPlaylist);
        }
        // MESSAGE CENTER END
    }
}
