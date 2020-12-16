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
using System.Reflection;
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

        private Boolean _CustomSongsOrder;

        public Boolean CustomSongsOrder
        {
            get { return _CustomSongsOrder; }
            set
            {
                _CustomSongsOrder = value;
                OnPropertyChanged(nameof(CustomSongsOrder));
            }
        }



        public ObservableCollection<SongViewModel> SongsList { get; private set; } = new ObservableCollection<SongViewModel>();

        public ObservableCollection<SongViewModel> SelectedSongs { get; private set; }
            = new ObservableCollection<SongViewModel>();


        private Boolean _AllowEditPlaylist;

        public Boolean AllowEditPlaylist
        {
            get { return _AllowEditPlaylist; }
            set
            {
                _AllowEditPlaylist = value;
                OnPropertyChanged(nameof(AllowEditPlaylist));
            }
        }

        //PROPERTIES SECTION END

        // VARIABLE SECTION START
        private readonly PlaylistRepository playlistRepository;
        private readonly IPageService _pageService;
        private readonly SongRepository songRepository;
        private readonly PositionRepository positionRepository;
        // VARIABLE SEXTION END

        // COMMAND START
        public ICommand SaveCommand { get; private set; }
        public ICommand DeletePlaylistCommand { get; private set; }
        public ICommand StartPresentationCommand { get; private set; }
        public ICommand ShowEditOptionsCommand { get; private set; }
        public ICommand DeleteSongsFromPlaylistCommand { get; private set; }
        public ICommand AddSongsToPlaylistCommand { get; private set; }
        public ICommand SelectAsToDeleteCommand { get; private set; }
        public ICommand UnselectAsToDeleteCommand { get; private set; }
        public ICommand MoveUpCommand { get; private set; }
        public ICommand MoveDownCommand { get; private set; }

        // COMMAND STOP

        // CONSTRUCTOR START
        public PlaylistDetailViewModel()
        {
            playlistRepository = new PlaylistRepository(DependencyService.Get<ISQLiteDb>());
            songRepository = new SongRepository(DependencyService.Get<ISQLiteDb>());
            positionRepository = new PositionRepository(DependencyService.Get<ISQLiteDb>());
            _pageService = new PageService();


            SaveCommand = new Command(async () => await Save());
            DeletePlaylistCommand = new Command(async () => await DeletePlaylist());
            StartPresentationCommand = new Command(async () => await StartPresentation());
            ShowEditOptionsCommand = new Command(ShowEditOptions);
            DeleteSongsFromPlaylistCommand = new Command(async songs => await DeleteSongsFromPlaylist());
            AddSongsToPlaylistCommand = new Command(async () => await AddSongsToPlaylist());

            SelectAsToDeleteCommand = new Command<SongViewModel>(async song => await SelectAsToDelete(song));
            UnselectAsToDeleteCommand = new Command<SongViewModel>(async song => await UnselectAsToDelete(song));

            MoveUpCommand = new Command(MoveUp);
            MoveDownCommand = new Command(MoveDown);

            MessagingCenter.Subscribe<PlaylistListViewModel, PlaylistViewModel>
            (this, Events.SendPlaylist, OnPlaylistSended);

            MessagingCenter.Subscribe<AddSongToPlaylistViewModel, ObservableCollection<SongViewModel>>
            (this, Events.AddSongsToPlaylist, OnSongsAdded);

            AllowEditPlaylist = false;
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
            try
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
                        List<Song> songsList = new List<Song>();
                        foreach (SongViewModel songViewModel in SongsList)
                        {
                            Song song = await songRepository.GetSongWithChildren(songViewModel.Id);
                            if (song.Positions == null)
                            {
                                song.Positions = new List<Position>();
                            }
                            else
                            {
                                Position checkIfPostionExist = song.Positions.Where(p => p.PlaylistId == Playlist.Id).FirstOrDefault();
                                if (checkIfPostionExist != null)
                                {
                                    checkIfPostionExist.PositionOnPlaylist = SongsList.IndexOf(songViewModel);
                                    await positionRepository.UpdatePosition(checkIfPostionExist);
                                }
                                else
                                {
                                    Position position = new Position() { PlaylistId = Playlist.Id, PositionOnPlaylist = SongsList.IndexOf(songViewModel) };
                                    await positionRepository.AddPosition(position);
                                    song.Positions.Add(position);
                                    await songRepository.UpdateSong(song);
                                }
                                songsList.Add(song);
                            }

                        }
                        Playlist.Songs = songsList;
                        await playlistRepository.UpdatePlaylist(Playlist);
                        MessagingCenter.Send(this, Events.PlaylistUpdated, Playlist);
                    }
                }
                await _pageService.PreviousDetailPage();
            }
            catch (TargetInvocationException e)
            {
                Console.WriteLine("Playlist save: " + e.InnerException.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Playlist save: " + e.Message);
            }

        }

        async Task StartPresentation()
        {
            await _pageService.ChangePageAsync(new SongTextPresentationView());
            MessagingCenter.Send(this, Events.SendPlaylistToPresentation, new PlaylistViewModel(Playlist));
        }

        void ShowEditOptions()
        {
            AllowEditPlaylist = !AllowEditPlaylist;

            foreach (SongViewModel song in SongsList)
            {
                song.IsCheckBoxVisible = AllowEditPlaylist;
            }
        }

        async Task DeleteSongsFromPlaylist()
        {
            foreach (SongViewModel song in SelectedSongs)
            {
                SongsList.Remove(song);
            }
        }
        async Task AddSongsToPlaylist()
        {
            await _pageService.ChangePageAsync(new AddSongToPlaylistView());
        }

        async Task SelectAsToDelete(SongViewModel song)
        {
            if (!SelectedSongs.Contains(song))
                SelectedSongs.Add(song);
        }

        async Task UnselectAsToDelete(SongViewModel song)
        {
            SelectedSongs.Remove(song);
        }

        async void MoveUp()
        {
            if (SelectedSongs.Count > 1)
            {
                await _pageService.DisplayAlert("Uwaga", "Próbujesz przenieść dwa lub więcej utworów, proszę przenosić pojedyńczo!", "OK");
            }
            else
            {
                int songPosition = SongsList.IndexOf(SelectedSongs[0]);
                if (songPosition != 0)
                {
                    SongsList.Move(songPosition, songPosition - 1);
                    Playlist.CustomSongsOrder = true;
                }
            }

        }

        async void MoveDown()
        {
            if (SelectedSongs.Count > 1)
            {
                await _pageService.DisplayAlert("Uwaga", "Próbujesz przenieść dwa lub więcej utworów, proszę przenosić pojedyńczo!", "OK");
            }
            else
            {
                int songPosition = SongsList.IndexOf(SelectedSongs[0]);
                if (songPosition != SongsList.Count - 1)
                {
                    SongsList.Move(songPosition, songPosition + 1);
                    Playlist.CustomSongsOrder = true;
                }
            }

        }
        // MESSAGE CENTER START
        private async void OnPlaylistSended(PlaylistListViewModel source, PlaylistViewModel playlistViewModel)
        {

            if (playlistViewModel != null)
            {
                ObservableCollection<SongViewModel> tempList = new ObservableCollection<SongViewModel>();
                Playlist = await playlistRepository.GetPlaylistWithSongs(playlistViewModel.Id);
                Name = Playlist.Name;
                foreach (Song songTemp in Playlist.Songs)
                {
                    if (Playlist.CustomSongsOrder)
                    {
                        Song tempSong = await songRepository.GetSongWithChildren(songTemp.Id);
                        tempSong.IsCheckBoxVisible = false;
                        tempList.Add(new SongViewModel(tempSong));
                    }
                    else
                    {
                        songTemp.IsCheckBoxVisible = false;
                        tempList.Add(new SongViewModel(songTemp));
                    }
                }
                if (Playlist.CustomSongsOrder)
                {
                    tempList = new ObservableCollection<SongViewModel>(tempList.OrderBy(s => s.Positions.Where(p => p.PlaylistId == Playlist.Id).FirstOrDefault().PositionOnPlaylist));
                }

                foreach (SongViewModel song in tempList)
                {
                    SongsList.Add(song);
                }
            }
            MessagingCenter.Unsubscribe<PlaylistListViewModel, PlaylistViewModel>(this, Events.SendPlaylist);
        }

        private async void OnSongsAdded(AddSongToPlaylistViewModel source, ObservableCollection<SongViewModel> songs)
        {
            foreach (SongViewModel song in songs)
            {
                song.IsChecked = false;
                SongsList.Add(song);
            }
            MessagingCenter.Unsubscribe<AddSongToPlaylistViewModel, ObservableCollection<SongViewModel>>(this, Events.AddSongsToPlaylist);
        }
        // MESSAGE CENTER END
    }
}
