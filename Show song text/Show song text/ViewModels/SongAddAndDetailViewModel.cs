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

            IsDeleteButtonVisible = false;
            Song = new Song();
            PageTitle = "Dodaj utwór";

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
            int searchType;
            FindedSongObject songToFind = new FindedSongObject() { Title = Title, Artist = Artist };



            if ((Title != null && !Title.Equals("")) && (Artist != null && !Artist.Equals("")))
            {
                songToFind.WorkingTitle = NormalizeTextWithoutPolishSpecialChar(Title.ToLower().Replace(" ", "_"));
                songToFind.WorkingArtist = NormalizeTextWithoutPolishSpecialChar(Artist.ToLower().Replace(" ", "_"));
                searchType = 1;
            }
            else if ((Artist != null && !Artist.Equals("")) && (Title == null || Title.Equals("")))
            {
                songToFind.WorkingArtist = NormalizeTextWithoutPolishSpecialChar(Artist.ToLower().Replace(" ", "_"));
                searchType = 2;
            }
            else if ((Artist == null || Artist.Equals("")) && (Title != null && !Title.Equals("")))
            {
                songToFind.WorkingTitle = NormalizeTextWithoutPolishSpecialChar(Title.ToLower().Replace(" ", "_"));
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
                    Text = await FindTextFromArtistAndTitle(songToFind);
                    break;
                case 2:
                    List<FindedSongObject> artistSongs = await SearchArtistSongs(songToFind);
                    if (artistSongs.Count == 0)
                    {
                        songToFind = await SearchArtist(songToFind);
                        List<FindedSongObject> findedSongs = await SearchArtistSongs(songToFind, true);
                        if (findedSongs.Count != 0)
                        {
                            List<string> listOfSongsFullName = new List<string>();
                            foreach (var item in findedSongs)
                            {
                                listOfSongsFullName.Add(item.FullSongName);
                            }
                            string choosenSong = await _pageService.DisplayPositionToChoose("Wybierz utwór:", "Anuluj", null, listOfSongsFullName.ToArray());
                            songToFind = findedSongs[listOfSongsFullName.IndexOf(choosenSong)];
                            Text = await FindTextFromArtistAndTitle(songToFind, true);
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
                        songToFind = artistSongs[listOfSongsFullName.IndexOf(choosenSong)];
                        Text = await FindTextFromArtistAndTitle(songToFind, true);
                        if (!String.IsNullOrEmpty(Text))
                        {
                            Title = songToFind.Title;
                        }
                    }
                    break;
                case 3:
                    songToFind = await SearchSong(songToFind);
                    Text = await FindTextFromArtistAndTitle(songToFind, true);
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

        // COMMAND METHOD END

        // METHOD FOR SEARCH TEXT START

        private async Task<string> FindTextFromArtistAndTitle(FindedSongObject songToFind, Boolean useLinkFromObject = false)
        {
            string html = null;
            string text = null;
            try
            {
                string url;
                if (useLinkFromObject && !String.IsNullOrEmpty(songToFind.LinkToSong))
                {
                    url = $"https://www.tekstowo.pl/" + songToFind.LinkToSong;
                }
                else if (songToFind.WorkingArtist != "" && songToFind.WorkingTitle != "")
                {
                    url = $"https://www.tekstowo.pl/piosenka,{songToFind.WorkingArtist},{songToFind.WorkingTitle}.html";

                }
                else { return null; }
                HttpClient httpClient = new HttpClient();
                html = await httpClient.GetStringAsync(url);


            }
            catch (HttpRequestException)
            {
                await _pageService.DisplayAlert("Nie znaleziono tekstu", "Nie udało sie znaleźć utworu dla: " + songToFind.Title + " " + songToFind.Artist, "OK");
                return null;
            }
            finally
            {

                if (html != null && html != "")
                {
                    HtmlDocument htmlDocument = new HtmlDocument();
                    htmlDocument.LoadHtml(html);

                    text = htmlDocument.DocumentNode.SelectSingleNode("//div[@id='songText']").InnerText;

                    text = DeleteLines(text, 2, false);
                    text = DeleteLines(text, 4, true);
                }

            }
            return text;
        }

        private async Task<List<FindedSongObject>> SearchArtistSongs(FindedSongObject songToFind, Boolean useLinkToSongs = false)
        {
            List<FindedSongObject> songs = new List<FindedSongObject>();
            string html = null;
            string artistWithFirstCharUpper = char.ToUpper(songToFind.Artist[0]) + songToFind.Artist.Substring(1);
            try
            {
                string url;
                if(useLinkToSongs && !String.IsNullOrEmpty(songToFind.LinkToArtistSongs))
                {
                    url = $"https://www.tekstowo.pl/" + songToFind.LinkToArtistSongs;
                }
                else if (!String.IsNullOrEmpty(songToFind.WorkingArtist))
                {
                    url = $"https://www.tekstowo.pl/piosenki_artysty,{songToFind.WorkingArtist}.html";
                }
                else { return null; }
                HttpClient httpClient = new HttpClient();
                html = await httpClient.GetStringAsync(url);
            }
            catch (HttpRequestException)
            {

                await _pageService.DisplayAlert("Problem przy wyszukiwaniu", "Nie udało sie znaleźć utworów artysty: " + artistWithFirstCharUpper, "OK");
                return null;
            }
            finally
            {
                HtmlDocument htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(html);

                HtmlNodeCollection query = htmlDocument.DocumentNode.SelectNodes("//div[@class='box-przeboje']");

                if (query != null)
                {
                    List<HtmlNode> findedSongs = query.ToList();

                    foreach (var div in findedSongs)
                    {
                        string[] divArtistSongNameDotSplit = div.InnerText.Split('.');
                        int indexStartLink = div.InnerHtml.IndexOf("<a href") + 10;
                        int indexEndLink = div.InnerHtml.IndexOf("class=\"title\" title=") - 2;
                        int indexStringToFindStart = div.InnerHtml.IndexOf("<span class=\"rank\">") + "<span class=\"rank\">".Length;
                        int indextStringToFindEnd = div.InnerHtml.IndexOf("</span>");
                        string stringToFind = div.InnerHtml.Substring(indexStringToFindStart, indextStringToFindEnd - indexStringToFindStart);
                        string link = div.InnerHtml.Substring(indexStartLink, indexEndLink - indexStartLink);
                        if (!String.IsNullOrEmpty(divArtistSongNameDotSplit[1]) && divArtistSongNameDotSplit[1].Contains(ReverseReplaceCodeOfCharOnSpeciaChar(songToFind.Artist)) && !String.IsNullOrEmpty(link))
                        {
                            Console.WriteLine(divArtistSongNameDotSplit[1]);
                            string fullSongName = divArtistSongNameDotSplit[1].Replace("\n", "").Replace("\r", "").Trim();
                            fullSongName = fullSongName.Substring(0, fullSongName.Length - stringToFind.Length).Trim();
                            string[] fullSongNameSplit = fullSongName.Split('-');
                            songs.Add(new FindedSongObject()
                            {
                                FullSongName = fullSongName,
                                Artist = ReplaceCodeOfCharOnSpeciaChar(fullSongNameSplit[0].Trim()),
                                Title = fullSongNameSplit[1].Trim(),
                                LinkToSong = link
                            });
                        }
                    }
                }
            }
            return songs;
        }

        private async Task<FindedSongObject> SearchArtist(FindedSongObject songToFind)
        {
            string html = null;
            string url = null;
            string artistWithFirstCharUpper = char.ToUpper(songToFind.Artist[0]) + songToFind.Artist.Substring(1);
            try
            {
                songToFind.WorkingArtist = songToFind.WorkingArtist.Replace("_", "+");
                if (!String.IsNullOrEmpty(songToFind.WorkingArtist))
                {
                    url = $"https://www.tekstowo.pl/szukaj,wykonawca,{songToFind.WorkingArtist},tytul.html";
                }
                HttpClient httpClient = new HttpClient();
                html = await httpClient.GetStringAsync(url);
            }
            catch (HttpRequestException)
            {

                await _pageService.DisplayAlert("Problem przy wyszukiwaniu", "Nie udało sie znaleźć pasujących artystów: " + artistWithFirstCharUpper, "OK");
                return null;
            }
            finally
            {
                HtmlDocument htmlDocumentSearchArtist = new HtmlDocument();
                htmlDocumentSearchArtist.LoadHtml(html);

                HtmlNodeCollection query = htmlDocumentSearchArtist.DocumentNode.SelectNodes("//div[@class='box-przeboje']");
                if (query != null)
                {
                    List<HtmlNode> findedArtist = query.ToList();

                    Dictionary<string, string> listOfArtist = new Dictionary<string, string>();
                    foreach (var div in findedArtist)
                    {
                        string[] divDotSplit = div.InnerText.Split('.');
                        string[] divLeftRoundBracketsSplit = divDotSplit[1].Split('(');
                        int indexStartLink = div.InnerHtml.IndexOf("<a href") + 10;
                        int indexEndLink = div.InnerHtml.IndexOf("class=\"title\" title=") - 2;
                        string link = div.InnerHtml.Substring(indexStartLink, indexEndLink - indexStartLink);
                        if (!String.IsNullOrEmpty(divLeftRoundBracketsSplit[0]))
                        {
                            Console.WriteLine(divLeftRoundBracketsSplit[0]);
                            divLeftRoundBracketsSplit[0] = divLeftRoundBracketsSplit[0].Replace("\n", "").Replace("\r", "");
                            divLeftRoundBracketsSplit[0] = divLeftRoundBracketsSplit[0].Trim();
                            divLeftRoundBracketsSplit[0] = ReplaceCodeOfCharOnSpeciaChar(divLeftRoundBracketsSplit[0]);
                            listOfArtist.Add(divLeftRoundBracketsSplit[0], link);
                        }
                    }

                    songToFind.Artist = await _pageService.DisplayPositionToChoose("Wybierz artystę:", "Anuluj", null, listOfArtist.Keys.ToArray());
                    songToFind.LinkToArtistSongs = listOfArtist[songToFind.Artist];
                    songToFind.WorkingArtist = NormalizeTextWithoutPolishSpecialChar(songToFind.Artist.ToLower().Replace(" ", "_"));
                }
            }
            return songToFind;

        }

        private async Task<FindedSongObject> SearchSong(FindedSongObject songToFind)
        {
            string html = null;
            string url = null;
            try
            {
                songToFind.WorkingTitle = songToFind.WorkingTitle.Replace("_", "+");
                if (!String.IsNullOrEmpty(songToFind.WorkingTitle))
                {
                    url = $"https://www.tekstowo.pl/szukaj,wykonawca,,tytul,{songToFind.WorkingTitle}.html";
                }
                HttpClient httpClient = new HttpClient();
                html = await httpClient.GetStringAsync(url);
            }
            catch (HttpRequestException)
            {

                await _pageService.DisplayAlert("Problem przy wyszukiwaniu", "Nie udało sie znaleźć pasujących artystów.", "OK");
                return null;
            }
            finally
            {
                HtmlDocument htmlDocumentSearchArtist = new HtmlDocument();
                htmlDocumentSearchArtist.LoadHtml(html);

                HtmlNodeCollection query = htmlDocumentSearchArtist.DocumentNode.SelectNodes("//div[@class='box-przeboje']");
                if (query != null)
                {
                    List<HtmlNode> findedSongs = query.ToList();

                    Dictionary<string, string> listOfSongs = new Dictionary<string, string>();
                    foreach (var div in findedSongs)
                    {
                        string[] divDotSplit = div.InnerText.Split(new char[] { '.' }, 2 );
                        int indexStartLink = div.InnerHtml.IndexOf("<a href") + 10;
                        int indexEndLink = div.InnerHtml.IndexOf("class=\"title\" title=") - 2;
                        string link = div.InnerHtml.Substring(indexStartLink, indexEndLink - indexStartLink);
                        if (!String.IsNullOrEmpty(divDotSplit[1]))
                        {
                            Console.WriteLine(divDotSplit[1]);
                            divDotSplit[1] = divDotSplit[1].Replace("\n", "").Replace("\r", "");
                            divDotSplit[1] = divDotSplit[1].Trim();
                            divDotSplit[1] = ReplaceCodeOfCharOnSpeciaChar(divDotSplit[1]);
                            listOfSongs.Add(divDotSplit[1], link);
                        }
                    }
                    string choosenSong = await _pageService.DisplayPositionToChoose("Wybierz utwór:", "Anuluj", null, listOfSongs.Keys.ToArray());
                    string[] choosenSongSplit = choosenSong.Split('-');
                    songToFind.Artist = choosenSongSplit[0].Trim();
                    songToFind.Title = choosenSongSplit[1].Trim();
                    songToFind.LinkToSong = listOfSongs[choosenSong];
                    songToFind.WorkingArtist = NormalizeTextWithoutPolishSpecialChar(songToFind.Title.ToLower().Replace(" ", "_"));
                    songToFind.WorkingTitle = NormalizeTextWithoutPolishSpecialChar(songToFind.Title.ToLower().Replace(" ", "_"));
                }
            }
            return songToFind;
        }

        // METHOD FOR SEARCH TEXT END

        // MESSAGE CENTER START

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
            }
            PageTitle = "Podgląd utworu";
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

        private string DeleteLines(string stringToRemoveLinesFrom,
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

        private string ReplaceCodeOfCharOnSpeciaChar(string text)
        {
            text = text.Replace("&amp;", "&");
            text = text.Replace("&#039;", "'");

            return text;
        }

        private string ReverseReplaceCodeOfCharOnSpeciaChar(string text)
        {
            text = text.Replace("&", "&amp;");
            text = text.Replace("'", "&#039;");

            return text;
        }
        // OTHER METHOD END


    }
}

