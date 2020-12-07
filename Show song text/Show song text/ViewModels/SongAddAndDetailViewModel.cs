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

namespace Show_song_text.ViewModels
{
    public class SongAddAndDetailViewModel : ViewModelBase
    {
        // VARIABLE  START
        private readonly SongRepository songRepository;
        private readonly IPageService _pageService;
        // VARIABLE  END

        // PROPERTY START
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

        public Boolean IsFloatingButtonVisible { get; set; }
        // PROPERTY END

        // COMMAND STRART
        public ICommand SaveCommand { get; private set; }
        public ICommand DeleteSongCommand { get; private set; }

        public ICommand SearchTextCommand { get; set; }
        // COMMAND END


        // CONSTRUCTOR START
        public SongAddAndDetailViewModel()
        {
            _pageService = new PageService();
            songRepository = new SongRepository(DependencyService.Get<ISQLiteDb>());

            MessagingCenter.Subscribe<SongListViewModel, SongViewModel>
            (this, Events.SendSong, OnSongSended);

            SaveCommand = new Command(async () => await Save());
            DeleteSongCommand = new Command(async () => await DeleteSong());
            SearchTextCommand = new Command(async () => await SearchText());

            IsFloatingButtonVisible = false;
            Song = new Song
            {
                Artist = Artist,
                Title = Title,
                Text = Text,
                Chords = Chords
            };

            //new Thread(new ThreadStart(delegate {
            //    AsynchronousClient.StartClient();
            //})).Start();

        }
        // CONSTRUCTOR END

        // COMMAND METHOD START
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
                    await _pageService.DisplayAlert("Bład", "Wprowadź nazwę i autora.", "OK");
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
        async Task SearchText()
        {

            string tempTitle = NormalizeTextWithoutPolishSpecialChar(Title);
            string tempArtist = NormalizeTextWithoutPolishSpecialChar(Artist);
            tempTitle = tempTitle.Replace(" ", "_");
            tempArtist = tempArtist.Replace(" ", "_");


            var url = $"https://www.tekstowo.pl/piosenka,{tempArtist},{tempTitle}.html";
            string html;
            HttpClient httpClient = new HttpClient();
            try
            {
                html = await httpClient.GetStringAsync(url);
            }
            catch (HttpRequestException)
            {
                await _pageService.DisplayAlert("Nie znaleziono tekstu", "Nie udało sie znaleźć utworu dla: " + tempTitle + " " + tempArtist + " " + url, "OK");
                return;
            }


            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            var text = htmlDocument.DocumentNode.SelectSingleNode("//div[@id='songText']").InnerText;

            text = DeleteLines(text, 2, false);
            text = DeleteLines(text, 4, true);

            Text = text;

        }

        // COMMAND METHOD END

        // MESSAGE CENTER START

        private void OnSongSended(SongListViewModel source, SongViewModel song)
        {
            if (song != null)
            {
                IsFloatingButtonVisible = true;
                Song = new Song
                {
                    Id = song.Id,
                    Artist = song.Artist,
                    Title = song.Title,
                    Text = song.Text,
                    Chords = song.Chords,
                    PlaylistID = song.Playlist,
                    IsCheckBoxVisible = song.IsCheckBoxVisible,
                    IsChecked = song.IsChecked
                };
                Title = Song.Title;
                Artist = Song.Artist;
                Text = Song.Text;
                Chords = Song.Chords;
            }
            MessagingCenter.Unsubscribe<SongListViewModel, SongViewModel>(this, Events.SendSong);
        }

        // MESSAGE CENTER END

        // OTHER METHOD START

        private string NormalizeTextWithoutPolishSpecialChar(string text)
        {
            var decomposed = text.Normalize(NormalizationForm.FormD);
            var filtered = decomposed.Where(c => char.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark);
            var newString = new String(filtered.ToArray());
            newString = newString.Replace("ł", "l");
            newString = newString.Replace("Ł", "L");

            return newString;
        }

        public static string DeleteLines(string stringToRemoveLinesFrom,
                                        int numberOfLinesToRemove,
                                        bool startFromBottom = false)
        {
            string toReturn = "";
            string[] allLines = stringToRemoveLinesFrom.Split(
                    separator: Environment.NewLine.ToCharArray(),
                    options: StringSplitOptions.RemoveEmptyEntries);
            if (startFromBottom)
                toReturn = String.Join(Environment.NewLine, allLines.Take(allLines.Length - numberOfLinesToRemove));
            else
                toReturn = String.Join(Environment.NewLine, allLines.Skip(numberOfLinesToRemove));
            return toReturn;
        }
        // OTHER METHOD END
    }
}

