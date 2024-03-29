﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using ShowSongText.Abstraction;
using ShowSongText.Models;
using ShowSongText.Resources.Languages;
using ShowSongText.Utils;

namespace ShowSongText.Helpers
{
    public class TekstowoHelper
    {
        public static async Task<string> FindTextFromArtistAndTitle(FindedSongObject songToFind, bool useLinkFromObject = false)
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

                await _pageService.DisplayAlert(AppResources.AlertDialog_TextNotFound, $"{AppResources.AlertDialog_TextNotFoundFor} {songToFind.Title} {songToFind.Artist} \n{AppResources.AlertDialog_SelectArtistAndTrackManually}", AppResources.AlertDialog_OK);
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
                    text = DeleteStartAndEndEmptyLines(text);
                }

            }
            return text;
        }

        public static async Task<List<FindedSongObject>> SearchArtistSongs(FindedSongObject songToFind, bool useLinkToSongs = false)
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
                await _pageService.DisplayAlert(AppResources.AlertDialog_SearchProblem, $"{AppResources.AlertDialog_CouldNotFindArtistsSongs} {artistWithFirstCharUpper}", AppResources.AlertDialog_OK);
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
                            string link = div.InnerHtml[indexStartLink..indexEndLink];
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

                            await _pageService.DisplayAlert(AppResources.AlertDialog_SearchProblem, $"{AppResources.AlertDialog_CouldNotFindArtistsSongs} {artistWithFirstCharUpper}", AppResources.AlertDialog_OK);
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
            string url = null;
            string artistWithFirstCharUpper = char.ToUpper(songToFind.Artist[0]) + songToFind.Artist.Substring(1);
            Dictionary<string, string> artistsList = new Dictionary<string, string>();
            string html;
            try
            {
                songToFind.WorkingArtist = songToFind.WorkingArtist.Replace("_", "+");
                songToFind.WorkingArtist = songToFind.WorkingArtist.Replace("/", "_");
                songToFind.WorkingArtist = songToFind.WorkingArtist.Replace("'", "%27");
                if (!String.IsNullOrEmpty(songToFind.WorkingArtist))
                {
                    url = $"https://www.tekstowo.pl/szukaj,wykonawca,{songToFind.WorkingArtist},tytul.html";
                }
                HttpClient httpClient = new HttpClient();
                html = await httpClient.GetStringAsync(url);
            }
            catch (HttpRequestException)
            {

                await _pageService.DisplayAlert(AppResources.AlertDialog_SearchProblem, $"{AppResources.AlertDialog_CouldntFindMachingArtists}: {artistWithFirstCharUpper}", AppResources.AlertDialog_OK);
                return null;
            }

            HtmlDocument htmlDocumentSearchArtist = new HtmlDocument();
            htmlDocumentSearchArtist.LoadHtml(html);

            HtmlNodeCollection queryCheckIfToMany = htmlDocumentSearchArtist.DocumentNode.SelectNodes("//li[@class='page-item disbled']");

            if (queryCheckIfToMany != null)
            {
                await _pageService.DisplayAlert(AppResources.AlertDialog_SearchProblem, AppResources.AlertDialog_ToManyMachingResults, AppResources.AlertDialog_OK);
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
                            string link = div.InnerHtml[indexStartLink..indexEndLink];
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

                            await _pageService.DisplayAlert(AppResources.AlertDialog_SearchProblem, $"{AppResources.AlertDialog_CouldntFindMachingArtists}.", AppResources.AlertDialog_OK);
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
                    else
                    {
                        await _pageService.DisplayAlert(AppResources.AlertDialog_CouldntFindMachingArtists, AppResources.AlertDialog_CheckNameAndTryAgain, AppResources.AlertDialog_OK);
                        return null;
                    }
                }
                if (artistsList.Count > 0)
                {
                    string choosenArtist = await _pageService.DisplayPositionToChoose(AppResources.DisplayPositionToChoose_ChooseArtist, AppResources.AlertDialog_Cancel, null, artistsList.Keys.ToArray());
                    if (choosenArtist.Equals(AppResources.AlertDialog_Cancel))
                    {
                        return null;
                    }
                    songToFind.Artist = choosenArtist;
                    songToFind.LinkToArtistSongs = artistsList[songToFind.Artist];
                    songToFind.WorkingArtist = NormalizeTextWithoutPolishSpecialChar(songToFind.Artist.ToLower().Replace(" ", "_"));
                }
                else
                {
                    return null;
                }

            }
            return songToFind;
        }


        public static async Task<FindedSongObject> SearchSong(FindedSongObject songToFind)
        {
            IPageService _pageService = new PageService();
            string url = null;
            Dictionary<string, string> listOfSongs = new Dictionary<string, string>();
            string html;
            try
            {
                songToFind.WorkingTitle = songToFind.WorkingTitle.Replace("_", "+");
                songToFind.WorkingTitle = songToFind.WorkingTitle.Replace("'", "%27");
                if (!String.IsNullOrEmpty(songToFind.WorkingTitle))
                {
                    url = $"https://www.tekstowo.pl/szukaj,wykonawca,,tytul,{songToFind.WorkingTitle}.html";
                }
                HttpClient httpClient = new HttpClient();
                html = await httpClient.GetStringAsync(url);
            }
            catch (HttpRequestException)
            {

                await _pageService.DisplayAlert(AppResources.AlertDialog_SearchProblem, $"{AppResources.AlertDialog_CouldntFindMachingArtists}.", AppResources.AlertDialog_OK);
                return null;
            }
            HtmlDocument htmlDocumentSearchSong = new HtmlDocument();
            htmlDocumentSearchSong.LoadHtml(html);

            HtmlNodeCollection queryCheckIfToMany = htmlDocumentSearchSong.DocumentNode.SelectNodes("//li[@class='page-item disbled']");

            if (queryCheckIfToMany != null)
            {
                await _pageService.DisplayAlert(AppResources.AlertDialog_SearchProblem, AppResources.AlertDialog_ToManyMachingResults, AppResources.AlertDialog_OK);
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
                        string link = div.InnerHtml[indexStartLink..indexEndLink];
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

                        await _pageService.DisplayAlert(AppResources.AlertDialog_SearchProblem, $"{AppResources.AlertDialog_CouldntFindMachingArtists}.", AppResources.AlertDialog_OK);
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
            string choosenSong = await _pageService.DisplayPositionToChoose(AppResources.DisplayPositionToChoose_ChooseSong, AppResources.AlertDialog_Cancel, null, listOfSongs.Keys.ToArray());
            if (choosenSong.Equals(AppResources.AlertDialog_Cancel))
            {
                return null;
            }
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
                    string link = div.InnerHtml[indexStartLink..indexEndLink];
                    string artist = div.InnerHtml[indexStartArtist..indexEndArtist];
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
                    string link = div.InnerHtml[indexStartLink..indexEndLink];

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

        private static List<FindedSongObject> GetArtistSongFromCurrentPage(HtmlDocument htmlDocument, FindedSongObject songToFind)
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
                    string link = div.InnerHtml[indexStartLink..indexEndLink];
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

        public static string NormalizeTextWithoutPolishSpecialChar(string text)
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
            string[] allLines = stringToRemoveLinesFrom.Split(
                    separator: Environment.NewLine.ToCharArray());
            if (startFromBottom)
                return String.Join(Environment.NewLine, allLines.Take(allLines.Length - numberOfLinesToRemove));
            else
                return String.Join(Environment.NewLine, allLines.Skip(numberOfLinesToRemove));
        }

        public static string ReplaceCodeOfCharOnSpeciaChar(string text)
        {
            text = text.Replace("&amp;", "&");
            text = text.Replace("&#039;", "'");

            return text;
        }

        public static string ReverseReplaceCodeOfCharOnSpeciaChar(string text)
        {
            text = text.Replace("&", "&amp;");
            text = text.Replace("'", "&#039;");

            return text;
        }

        public static string DeleteStartAndEndEmptyLines(string text)
        {
            List<string> textLines = text.Split(
                   separator: Environment.NewLine.ToCharArray()).ToList();
            List<string> readyText = text.Split(
                   separator: Environment.NewLine.ToCharArray()).ToList();

            for (int i = 0; i < textLines.Count; i++)
            {
                if (String.IsNullOrWhiteSpace(textLines[i]))
                {
                    readyText.Remove(textLines[i]);
                }
                else
                {
                    break;
                }
            }
            for (int i = textLines.Count - 1; i > 0; i--)
            {
                if (String.IsNullOrWhiteSpace(textLines[i]))
                {
                    readyText.Remove(textLines[i]);
                }
                else
                {
                    break;
                }
            }
            return String.Join(Environment.NewLine, readyText.ToArray()); ;
        }
        // OTHER METHOD END
    }
}
