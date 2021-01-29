using Show_song_text.Database.DOA;
using Show_song_text.Database.Models;
using Show_song_text.Database.ViewModels;
using Show_song_text.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Show_song_text.Utils;
using Show_song_text.Database.Repository;
using Show_song_text.Database.Persistence;
using System.Net.Http;
using HtmlAgilityPack;
using System.Linq;
using System.Globalization;
using Show_song_text.Views;
using System.Threading;
using Show_song_text.PresentationServerUtilis;
using Show_song_text.Models;
using Show_song_text.Helpers;

namespace Show_song_text.ViewModels
{
    public class SongAddAndDetailViewModel : ViewModelBase
    {
        #region Property
        public Song Song { get; private set; }
        private string _title;
        public string Title
        {
            get
            {
                return _title;

            }
            set
            {
                _title = value;
                Song.Title = value;
                OnPropertyChanged(nameof(Title));
            }
        }

        private string _artist;
        public string Artist
        {
            get
            {
                return _artist;

            }
            set
            {
                _artist = value;
                Song.Artist = value;
                OnPropertyChanged(nameof(Artist));
            }
        }

        private string _text;
        public string Text
        {
            get
            {
                return _text;

            }
            set
            {
                _text = value;
                Song.Text = value;
                OnPropertyChanged(nameof(Text));
            }
        }

        private string _chords;
        public string Chords
        {
            get
            {
                return _chords;

            }
            set
            {
                _chords = value;
                Song.Chords = value;
                OnPropertyChanged(nameof(Chords));
            }
        }

        private string _PageTitle;

        public string PageTitle
        {
            get { return _PageTitle; }
            set
            {
                _PageTitle = value;
                OnPropertyChanged(nameof(PageTitle));
            }
        }
        private Boolean _IsDeleteButtonVisible;

        public Boolean IsDeleteButtonVisible
        {
            get { return _IsDeleteButtonVisible; }
            set
            {
                _IsDeleteButtonVisible = value;
                OnPropertyChanged(nameof(IsDeleteButtonVisible));
            }
        }

        private List<Playlist> _Playlists;

        public List<Playlist> Playlists
        {
            get { return _Playlists; }
            set
            {
                _Playlists = value;
                OnPropertyChanged(nameof(Playlists));
            }
        }

        private Playlist _selectedPlaylist;

        public Playlist SelectedPlaylist
        {
            get { return _selectedPlaylist; }
            set
            {
                _selectedPlaylist = value;
                OnPlaylistSelected(new PlaylistViewModel(value));
                OnPropertyChanged(nameof(SelectedPlaylist));
            }
        }

        #endregion

        #region Variable
        private readonly SongRepository songRepository;
        private readonly IPageService _pageService;
        #endregion

        #region Commands
        public ICommand SaveCommand { get; private set; }
        public ICommand DeleteSongCommand { get; private set; }

        public ICommand SearchTextCommand { get; set; }
        public ICommand SelectPlaylistCommand { get; private set; }
        #endregion


        #region Constructor
        public SongAddAndDetailViewModel()
        {
            _pageService = new PageService();
            songRepository = new SongRepository(DependencyService.Get<ISQLiteDb>());

            MessagingCenter.Subscribe<SongListViewModel, SongViewModel>
            (this, Events.SendSong, OnSongSended);

            SaveCommand = new Command(async () => await Save());
            DeleteSongCommand = new Command(async () => await DeleteSong());
            SearchTextCommand = new Command(async () => await SearchText());
            SelectPlaylistCommand = new Command<PlaylistViewModel>(async playlist => await SelectPlaylist(playlist));

            IsDeleteButtonVisible = false;
            Song = new Song();
            PageTitle = "Dodaj utwór";

        }
        #endregion

        #region Commands methods
        private async Task DeleteSong()
        {
            if (await _pageService.DisplayAlert("Ostrzeżenie", $"Jesteś pewny, że chcesz usunąć {Song.Artist} {Song.Title}?", "Tak", "Nie"))
            {

                var songToDelete = await songRepository.GetSong(Song.Id);
                await songRepository.DeleteSong(songToDelete);
                MessagingCenter.Send(this, Events.SongDeleted, Song);
                _pageService.ChangePage(new SongListView());
            }
        }

        async Task Save()
        {
            if (Song == null || Song.Id == 0)
            {
                await songRepository.AddSong(Song);
                MessagingCenter.Send(this, Events.SongAdded, Song);
            }
            else if (Song.Id != 0)
            {
                if (String.IsNullOrWhiteSpace(Song.Title) && String.IsNullOrWhiteSpace(Song.Artist))
                {
                    await _pageService.DisplayAlert("Błąd", "Wprowadź nazwę i autora.", "OK");
                    return;
                }
                else
                {
                    await songRepository.UpdateSong(Song);
                    MessagingCenter.Send(this, Events.SongUpdated, Song);
                }
            }
            await _pageService.PreviousDetailPage();
        }

        private async Task SelectPlaylist(PlaylistViewModel playlist)
        {
            if (playlist == null)
                return;
            await _pageService.ChangePageAsync(new PlaylistDetailView());
            MessagingCenter.Send(this, Events.SendPlaylist, playlist);
            SelectedPlaylist = null;
        }
        async Task SearchText()
        {
            int searchType;
            FindedSongObject songToFind = new FindedSongObject() { Title = Title, Artist = Artist };



            if ((Title != null && !Title.Equals("")) && (Artist != null && !Artist.Equals("")))
            {
                songToFind.WorkingTitle = TekstowoHelper.NormalizeTextWithoutPolishSpecialChar(Title.ToLower().Replace(" ", "_"));
                songToFind.WorkingArtist = TekstowoHelper.NormalizeTextWithoutPolishSpecialChar(Artist.ToLower().Replace(" ", "_"));
                searchType = 1;
            }
            else if ((Artist != null && !Artist.Equals("")) && (Title == null || Title.Equals("")))
            {
                songToFind.WorkingArtist = TekstowoHelper.NormalizeTextWithoutPolishSpecialChar(Artist.ToLower().Replace(" ", "_"));
                searchType = 2;
            }
            else if ((Artist == null || Artist.Equals("")) && (Title != null && !Title.Equals("")))
            {
                songToFind.WorkingTitle = TekstowoHelper.NormalizeTextWithoutPolishSpecialChar(Title.ToLower().Replace(" ", "_"));
                searchType = 3;
            }
            else
            {
                await _pageService.DisplayAlert("Błąd", "Nieoczekiwany przypadek", "OK");
                return;
            }

            switch (searchType)
            {
                case 1:
                    Text = await TekstowoHelper.FindTextFromArtistAndTitle(songToFind);
                    if(Text == null)
                        goto case 2;
                    else
                        break;
                case 2:
                    List<FindedSongObject> artistSongs = await TekstowoHelper.SearchArtistSongs(songToFind);
                    if (artistSongs.Count == 0)
                    {
                        songToFind = await TekstowoHelper.SearchArtist(songToFind);
                        if (songToFind == null)
                        {
                            break;
                        }
                        List<FindedSongObject> findedSongs = await TekstowoHelper.SearchArtistSongs(songToFind, true);
                        if (findedSongs.Count != 0)
                        {
                            List<string> listOfSongsFullName = new List<string>();
                            foreach (var item in findedSongs)
                            {
                                listOfSongsFullName.Add(item.FullSongName);
                            }
                            string choosenSong = await _pageService.DisplayPositionToChoose("Wybierz utwór:", "Anuluj", null, listOfSongsFullName.ToArray());
                            if (choosenSong.Equals("Anuluj")){
                                break;
                            }
                            songToFind = findedSongs[listOfSongsFullName.IndexOf(choosenSong)];
                            Text = await TekstowoHelper.FindTextFromArtistAndTitle(songToFind, true);
                            if (!String.IsNullOrEmpty(Text))
                            {
                                Title = songToFind.Title;
                                Artist = songToFind.Artist;
                            }
                        }
                    }
                    else
                    {
                        List<string> listOfSongsFullName = new List<string>();
                        foreach (var item in artistSongs)
                        {
                            listOfSongsFullName.Add(item.FullSongName);
                        }
                        string choosenSong = await _pageService.DisplayPositionToChoose("Wybierz utwór:", "Anuluj", null, listOfSongsFullName.ToArray());
                        if (choosenSong.Equals("Anuluj")){
                            break;
                        }
                        songToFind = artistSongs[listOfSongsFullName.IndexOf(choosenSong)];
                        Text = await TekstowoHelper.FindTextFromArtistAndTitle(songToFind, true);
                        if (!String.IsNullOrEmpty(Text))
                        {
                            Title = songToFind.Title;
                        }
                    }
                    break;
                case 3:
                    songToFind = await TekstowoHelper.SearchSong(songToFind);
                    if (songToFind == null)
                        break;
                    Text = await TekstowoHelper.FindTextFromArtistAndTitle(songToFind, true);
                    if (!String.IsNullOrEmpty(Text))
                    {
                        Title = songToFind.Title;
                        Artist = songToFind.Artist;
                    }

                    break;
                default:
                    break;
            }



        }
        #endregion


        #region Message center methods
        private async void OnSongSended(SongListViewModel source, SongViewModel song)
        {
            if (song != null)
            {
                IsDeleteButtonVisible = true;
                Song = await songRepository.GetSongWithChildren(song.Id);
                Title = Song.Title;
                Artist = Song.Artist;
                Text = Song.Text;
                Chords = Song.Chords;
                Playlists = Song.Playlists;
            }
            PageTitle = "Podgląd utworu";
            MessagingCenter.Unsubscribe<SongListViewModel, SongViewModel>(this, Events.SendSong);
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

