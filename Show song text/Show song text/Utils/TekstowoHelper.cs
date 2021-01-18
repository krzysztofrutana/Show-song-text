using HtmlAgilityPack;
using Show_song_text.Interfaces;
using Show_song_text.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Globalization;

namespace Show_song_text.Utils
{
    public class TekstowoHelper
    {
        public static async Task<string> FindTextFromArtistAndTitle(FindedSongObject songToFind, Boolean useLinkFromObject = false)
        {
            IPageService _pageService = new PageService();
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
                
                await _pageService.DisplayAlert("Nie znaleziono tekstu", "Nie udało sie znaleźć utworu dla: " + songToFind.Title + " " + songToFind.Artist + "\nProszę ręcznie wybrać artyste i utwór.", "OK");
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

        public static async Task<List<FindedSongObject>> SearchArtistSongs(FindedSongObject songToFind, Boolean useLinkToSongs = false)
        {
            IPageService _pageService = new PageService();
            List<FindedSongObject> songs = new List<FindedSongObject>();
            string html = null;
            string artistWithFirstCharUpper = char.ToUpper(songToFind.Artist[0]) + songToFind.Artist.Substring(1);
            string url = "";
            try
            {

                if (useLinkToSongs && !String.IsNullOrEmpty(songToFind.LinkToArtistSongs))
                {
                    url = $"https://www.tekstowo.pl/{songToFind.LinkToArtistSongs}";
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

                // chech if is more than one pages of results

                HtmlNodeCollection queryPages = htmlDocument.DocumentNode.SelectNodes("//li[@class='page-item']");
                if (queryPages != null)
                {
                    var listOfPages = queryPages.ToList();
                    List<string> pagesHTMLLink = new List<string>();
                    if (!String.IsNullOrEmpty(url))
                        pagesHTMLLink.Add(url);

                    foreach (var div in listOfPages)
                    {
                        int indexStartLink = div.InnerHtml.IndexOf("href") + 7;
                        int indexEndLink = div.InnerHtml.IndexOf("title=") - 2;
                        int checkIndexOfNextPageButtonElement = div.InnerHtml.IndexOf("tabindex=");
                        if (checkIndexOfNextPageButtonElement != -1)
                            continue;
                        if (indexStartLink != 0 && indexEndLink != 0)
                        {
                            string link = div.InnerHtml.Substring(indexStartLink, indexEndLink - indexStartLink);
                            if (!String.IsNullOrEmpty(link))
                            {
                                Console.WriteLine(link);
                                if (!pagesHTMLLink.Contains($"https://www.tekstowo.pl/{link}"))
                                {
                                    pagesHTMLLink.Add($"https://www.tekstowo.pl/{link}");
                                }


                            }

                        }
                    }

                    foreach (string urlLink in pagesHTMLLink)
                    {
                        HttpClient httpClient = new HttpClient();
                        string htmlTemp;
                        try
                        {
                            htmlTemp = await httpClient.GetStringAsync(urlLink);
                        }
                        catch (HttpRequestException)
                        {

                            await _pageService.DisplayAlert("Problem przy wyszukiwaniu", "Nie udało sie znaleźć utworów artysty: " + artistWithFirstCharUpper, "OK");
                            continue;
                        }

                        HtmlDocument htmlDocumentTemp = new HtmlDocument();
                        htmlDocumentTemp.LoadHtml(htmlTemp);

                        List<FindedSongObject> findedSongs = GetArtistSongFromCurrentPage(htmlDocumentTemp, songToFind);
                        if (findedSongs != null)
                        {
                            foreach (FindedSongObject findedSongObjectTemp in findedSongs)
                            {
                                songs.Add(findedSongObjectTemp);
                            }
                        }

                    }
                }
                else
                {
                    List<FindedSongObject> findedSongs = GetArtistSongFromCurrentPage(htmlDocument, songToFind);
                    if (findedSongs != null)
                    {
                        songs = findedSongs;
                    }
                }
            }
            return songs;
        }

        public static async Task<FindedSongObject> SearchArtist(FindedSongObject songToFind)
        {
            IPageService _pageService = new PageService();
            string html = null;
            string url = null;
            string artistWithFirstCharUpper = char.ToUpper(songToFind.Artist[0]) + songToFind.Artist.Substring(1);
            Dictionary<string, string> artistsList = new Dictionary<string, string>();
            try
            {
                songToFind.WorkingArtist = songToFind.WorkingArtist.Replace("_", "+");
                songToFind.WorkingArtist = songToFind.WorkingArtist.Replace("/", "_");
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

            HtmlDocument htmlDocumentSearchArtist = new HtmlDocument();
            htmlDocumentSearchArtist.LoadHtml(html);

            HtmlNodeCollection queryCheckIfToMany = htmlDocumentSearchArtist.DocumentNode.SelectNodes("//li[@class='page-item disbled']");

            if (queryCheckIfToMany != null)
            {
                await _pageService.DisplayAlert("Problem przy wyszukiwaniu", "Za dużo pasujacych wyników, sprecyzuj wyszukiwanie!", "OK");
                return null;
            }
            else
            {
                HtmlNodeCollection queryPages = htmlDocumentSearchArtist.DocumentNode.SelectNodes("//li[@class='page-item']");
                if (queryPages != null)
                {
                    var listOfPages = queryPages.ToList();
                    List<string> pagesHTMLLink = new List<string>();
                    if (!String.IsNullOrEmpty(url))
                        pagesHTMLLink.Add(url);

                    foreach (var div in listOfPages)
                    {
                        int indexStartLink = div.InnerHtml.IndexOf("href") + 7;
                        int indexEndLink = div.InnerHtml.IndexOf("title=") - 2;
                        int checkIndexOfNextPageButtonElement = div.InnerHtml.IndexOf("tabindex=");
                        if (checkIndexOfNextPageButtonElement != -1)
                            continue;
                        if (indexStartLink != 0 && indexEndLink != 0)
                        {
                            string link = div.InnerHtml.Substring(indexStartLink, indexEndLink - indexStartLink);
                            if (!String.IsNullOrEmpty(link))
                            {
                                Console.WriteLine(link);
                                if (!pagesHTMLLink.Contains($"https://www.tekstowo.pl/{link}"))
                                {
                                    pagesHTMLLink.Add($"https://www.tekstowo.pl/{link}");
                                }
                            }

                        }
                    }

                    foreach (string urlLink in pagesHTMLLink)
                    {
                        HttpClient httpClient = new HttpClient();
                        string htmlTemp;
                        try
                        {
                            htmlTemp = await httpClient.GetStringAsync(urlLink);
                        }
                        catch (HttpRequestException)
                        {

                            await _pageService.DisplayAlert("Problem przy wyszukiwaniu", "Nie udało sie znaleźć pasujących artystów.", "OK");
                            continue;
                        }

                        HtmlDocument htmlDocumentTemp = new HtmlDocument();
                        htmlDocumentTemp.LoadHtml(htmlTemp);

                        Dictionary<string, string> tempListOfSongs = GetArtistFromCurrentPage(htmlDocumentTemp);
                        if (tempListOfSongs.Count != 0)
                        {
                            foreach (var item in tempListOfSongs)
                            {
                                artistsList.Add(item.Key, item.Value);
                            }
                        }

                    }

                }
                else
                {
                    Dictionary<string, string> tempListOfSongs = GetArtistFromCurrentPage(htmlDocumentSearchArtist);
                    if (tempListOfSongs.Count != 0)
                    {
                        foreach (var item in tempListOfSongs)
                        {
                            artistsList.Add(item.Key, item.Value);
                        }
                    }
                }


                songToFind.Artist = await _pageService.DisplayPositionToChoose("Wybierz artystę:", "Anuluj", null, artistsList.Keys.ToArray());
                songToFind.LinkToArtistSongs = artistsList[songToFind.Artist];
                songToFind.WorkingArtist = NormalizeTextWithoutPolishSpecialChar(songToFind.Artist.ToLower().Replace(" ", "_"));
            }

            return songToFind;
        }


        public static async Task<FindedSongObject> SearchSong(FindedSongObject songToFind)
        {
            IPageService _pageService = new PageService();
            string html = null;
            string url = null;
            List<FindedSongObject> songs = new List<FindedSongObject>();
            Dictionary<string, string> listOfSongs = new Dictionary<string, string>();
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
            HtmlDocument htmlDocumentSearchSong = new HtmlDocument();
            htmlDocumentSearchSong.LoadHtml(html);

            HtmlNodeCollection queryCheckIfToMany = htmlDocumentSearchSong.DocumentNode.SelectNodes("//li[@class='page-item disbled']");

            if (queryCheckIfToMany != null)
            {
                await _pageService.DisplayAlert("Problem przy wyszukiwaniu", "Za dużo pasujacych wyników, sprecyzuj wyszukiwanie!", "OK");
                return null;
            }

            HtmlNodeCollection queryPages = htmlDocumentSearchSong.DocumentNode.SelectNodes("//li[@class='page-item']");
            if (queryPages != null)
            {
                var listOfPages = queryPages.ToList();
                List<string> pagesHTMLLink = new List<string>();
                if (!String.IsNullOrEmpty(url))
                    pagesHTMLLink.Add(url);

                foreach (var div in listOfPages)
                {
                    int indexStartLink = div.InnerHtml.IndexOf("href") + 7;
                    int indexEndLink = div.InnerHtml.IndexOf("title=") - 2;
                    int checkIndexOfNextPageButtonElement = div.InnerHtml.IndexOf("tabindex=");
                    if (checkIndexOfNextPageButtonElement != -1)
                        continue;
                    if (indexStartLink != 0 && indexEndLink != 0)
                    {
                        string link = div.InnerHtml.Substring(indexStartLink, indexEndLink - indexStartLink);
                        if (!String.IsNullOrEmpty(link))
                        {
                            Console.WriteLine(link);
                            if (!pagesHTMLLink.Contains($"https://www.tekstowo.pl/{link}"))
                            {
                                pagesHTMLLink.Add($"https://www.tekstowo.pl/{link}");
                            }
                        }

                    }
                }

                foreach (string urlLink in pagesHTMLLink)
                {
                    HttpClient httpClient = new HttpClient();
                    string htmlTemp;
                    try
                    {
                        htmlTemp = await httpClient.GetStringAsync(urlLink);
                    }
                    catch (HttpRequestException)
                    {

                        await _pageService.DisplayAlert("Problem przy wyszukiwaniu", "Nie udało sie znaleźć pasujących artystów.", "OK");
                        continue;
                    }

                    HtmlDocument htmlDocumentTemp = new HtmlDocument();
                    htmlDocumentTemp.LoadHtml(htmlTemp);

                    Dictionary<string, string> tempListOfSongs = GetFindedSongsFromCurrentPage(htmlDocumentTemp);
                    if (tempListOfSongs.Count != 0)
                    {
                        foreach (var item in tempListOfSongs)
                        {
                            listOfSongs.Add(item.Key, item.Value);
                        }
                    }

                }

            }
            else
            {
                Dictionary<string, string> tempListOfSongs = GetFindedSongsFromCurrentPage(htmlDocumentSearchSong);
                if (tempListOfSongs.Count != 0)
                {
                    foreach (var item in tempListOfSongs)
                    {
                        listOfSongs.Add(item.Key, item.Value);
                    }
                }
            }
            string choosenSong = await _pageService.DisplayPositionToChoose("Wybierz utwór:", "Anuluj", null, listOfSongs.Keys.ToArray());
            string[] choosenSongSplit = choosenSong.Split('-');
            songToFind.Artist = choosenSongSplit[0].Trim();
            songToFind.Title = choosenSongSplit[1].Trim();
            songToFind.LinkToSong = listOfSongs[choosenSong];
            songToFind.WorkingArtist = NormalizeTextWithoutPolishSpecialChar(songToFind.Title.ToLower().Replace(" ", "_"));
            songToFind.WorkingTitle = NormalizeTextWithoutPolishSpecialChar(songToFind.Title.ToLower().Replace(" ", "_"));


            return songToFind;
        }

        private static Dictionary<string, string> GetArtistFromCurrentPage(HtmlDocument htmlDocument)
        {
            Dictionary<string, string> tempArtistsList = new Dictionary<string, string>();

            HtmlNodeCollection query = htmlDocument.DocumentNode.SelectNodes("//div[@class='box-przeboje']");
            if (query != null)
            {
                List<HtmlNode> findedArtist = query.ToList();

                foreach (var div in findedArtist)
                {
                    int indexStartLink = div.InnerHtml.IndexOf("<a href") + 10;
                    int indexEndLink = div.InnerHtml.IndexOf("class=\"title\" title=") - 2;
                    int indexStartArtist = div.InnerHtml.IndexOf("title=\"") + 7;
                    int indexEndArtist = div.InnerHtml.IndexOf("\">");
                    string link = div.InnerHtml.Substring(indexStartLink, indexEndLink - indexStartLink);
                    string artist = div.InnerHtml.Substring(indexStartArtist, indexEndArtist - indexStartArtist);
                    if (!String.IsNullOrEmpty(artist))
                    {
                        Console.WriteLine(artist);
                        artist = artist.Trim();
                        artist = ReplaceCodeOfCharOnSpeciaChar(artist);
                        tempArtistsList.Add(artist, link);
                    }
                }
            }
            return tempArtistsList;
        }
        private static Dictionary<string, string> GetFindedSongsFromCurrentPage(HtmlDocument htmlDocument)
        {
            Dictionary<string, string> tempFindedSongs = new Dictionary<string, string>();
            HtmlNodeCollection query = htmlDocument.DocumentNode.SelectNodes("//div[@class='box-przeboje']");
            if (query != null)
            {
                List<HtmlNode> findedSongs = query.ToList();


                foreach (var div in findedSongs)
                {
                    int indexStartLink = div.InnerHtml.IndexOf("<a href") + 10;
                    int indexEndLink = div.InnerHtml.IndexOf("class=\"title\" title=") - 2;
                    int indexStartTitle = div.InnerHtml.IndexOf("title=\"") + 7;
                    string tempTitle = div.InnerHtml.Substring(indexStartTitle);
                    int indexEndTitle = tempTitle.IndexOf("\">");
                    string title = tempTitle.Substring(0, indexEndTitle);
                    string link = div.InnerHtml.Substring(indexStartLink, indexEndLink - indexStartLink);

                    if (!String.IsNullOrEmpty(title))
                    {
                        Console.WriteLine(title);
                        title = title.Trim();
                        title = ReplaceCodeOfCharOnSpeciaChar(title);
                        tempFindedSongs.Add(title, link);
                    }
                }
            }
            return tempFindedSongs;
        }

        private static  List<FindedSongObject> GetArtistSongFromCurrentPage(HtmlDocument htmlDocument, FindedSongObject songToFind)
        {
            HtmlNodeCollection querySongs = htmlDocument.DocumentNode.SelectNodes("//div[@class='box-przeboje']");

            if (querySongs != null)
            {
                List<HtmlNode> findedSongs = querySongs.ToList();
                List<FindedSongObject> songs = new List<FindedSongObject>();
                foreach (var div in findedSongs)
                {
                    int indexStartLink = div.InnerHtml.IndexOf("<a href") + 10;
                    int indexEndLink = div.InnerHtml.IndexOf("class=\"title\" title=") - 2;
                    int indexStartArtistTitle = div.InnerHtml.IndexOf("class=\"title\" title=\"") + 21;
                    string tempArtistTitle = div.InnerHtml.Substring(indexStartArtistTitle);
                    int indexEndTitle = tempArtistTitle.IndexOf("\">");
                    string artistTitle = tempArtistTitle.Substring(0, indexEndTitle);
                    string link = div.InnerHtml.Substring(indexStartLink, indexEndLink - indexStartLink);
                    if (!String.IsNullOrEmpty(artistTitle) && artistTitle.Contains(ReverseReplaceCodeOfCharOnSpeciaChar(songToFind.Artist)) && !String.IsNullOrEmpty(link))
                    {
                        Console.WriteLine(artistTitle);
                        string fullSongName = artistTitle.Trim();
                        string[] fullSongNameSplit = fullSongName.Split('-');
                        songs.Add(new FindedSongObject()
                        {
                            FullSongName = ReplaceCodeOfCharOnSpeciaChar(fullSongName),
                            Artist = ReplaceCodeOfCharOnSpeciaChar(fullSongNameSplit[0].Trim()),
                            Title = ReplaceCodeOfCharOnSpeciaChar(fullSongNameSplit[1].Trim()),
                            LinkToSong = link
                        });
                    }
                }
                return songs;
            }
            else
            {
                return null;
            }
        }



        // OTHER METHOD START

        public  static  string NormalizeTextWithoutPolishSpecialChar(string text)
        {
            var decomposed = text.Normalize(NormalizationForm.FormD);
            var filtered = decomposed.Where(c => char.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark);
            var newString = new String(filtered.ToArray());
            newString = newString.Replace("ł", "l");
            newString = newString.Replace("Ł", "L");

            return newString;
        }

        private static  string DeleteLines(string stringToRemoveLinesFrom,
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

        private static string ReplaceCodeOfCharOnSpeciaChar(string text)
        {
            text = text.Replace("&amp;", "&");
            text = text.Replace("&#039;", "'");

            return text;
        }

        private static string ReverseReplaceCodeOfCharOnSpeciaChar(string text)
        {
            text = text.Replace("&", "&amp;");
            text = text.Replace("'", "&#039;");

            return text;
        }
        // OTHER METHOD END
    }
}
