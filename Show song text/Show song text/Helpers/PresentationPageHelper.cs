using System;
using ShowSongText.Models;
using Xamarin.Forms;
using XamarinLabelFontSizer;

namespace ShowSongText.Helpers
{
    public static class PresentationPageHelper
    {

        private static double FontSize { get; set; }
        public static PresentationPageModel PresentationPageModel { get; set; }

        public static Boolean CheckIfFit(Label label, double fontSize)
        {
            FontCalc lowerFontCalc = new FontCalc(label, fontSize, App.ScreenWidth * 0.9);
            if (lowerFontCalc.TextHeight > App.ScreenHeight * 0.80)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static Boolean CheckIfFitWithChordsOnRight(Label label, double fontSize)
        {
            FontCalc lowerFontCalc = new FontCalc(label, fontSize, (App.ScreenWidth * 0.9) * 0.85);
            if (lowerFontCalc.TextHeight > App.ScreenHeight * 0.80)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static Boolean CheckIfWrap(Label label, double fontSize)
        {
            FontCalc lowerFontCalc = new FontCalc(label, fontSize, Double.PositiveInfinity);
            if (lowerFontCalc.TextWidth > (App.ScreenWidth * 0.9) * 0.85)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        public static int GetFitPageModel(string[] textToFit, Label testLabel, string songTitle, Double fontSize, string songKey, bool addChords = false, string[] chordsToFit = null)
        {
            if (addChords)
            {
                FontSize = fontSize;

                int songTextLines = textToFit.Length;
                int songChordsLines = chordsToFit.Length;

                int linesCount = songTextLines + songChordsLines;
                PresentationPageModel = new PresentationPageModel();
                try
                {
                    for (int i = linesCount - 1; i >= 0; i -= 2)
                    {
                        int k = 0;
                        string[] temp = new string[i + 1];
                        for (int j = 0; j <= i; j += 2)
                        {
                            temp[j] = chordsToFit[k];
                            temp[j + 1] = textToFit[k];
                            temp[j + 1] = temp[j + 1].Trim();
                            k++;

                        }
                        testLabel.Text = string.Join(Environment.NewLine.ToString(), temp);
                        Boolean isFit = CheckIfFit(testLabel, FontSize);
                        if (isFit)
                        {
                            linesCount = k;
                            PresentationPageModel.Title = songTitle;
                            PresentationPageModel.Text = testLabel.Text;
                            PresentationPageModel.SongKey = songKey;
                            if (FontSize != 20)
                            {
                                PresentationPageModel.FontSize = FontSize;
                            }
                            break;

                        }
                    }

                }
                catch (IndexOutOfRangeException e)
                {
                    Console.WriteLine(e.Message);
                }
                if (!String.IsNullOrEmpty(PresentationPageModel.Text) && !String.IsNullOrEmpty(PresentationPageModel.Title))
                {
                    return linesCount;
                }
                else
                {
                    return -1;
                }

            }
            else
            {
                FontSize = fontSize;

                int songTextLines = textToFit.Length;

                if (songTextLines == 1 && String.IsNullOrWhiteSpace(textToFit[0]))
                {
                    return songTextLines;
                }

                int linesCount = songTextLines;
                PresentationPageModel = new PresentationPageModel();
                for (int i = songTextLines - 1; i >= 0; i--)
                {
                    string[] temp = new string[i + 1];
                    for (int j = 0; j <= i; j++)
                    {
                        temp[j] = textToFit[j];
                        temp[j] = temp[j].Trim();
                    }
                    testLabel.Text = string.Join(Environment.NewLine.ToString(), temp);
                    Boolean isFit = CheckIfFit(testLabel, FontSize);
                    if (isFit)
                    {
                        linesCount = i;
                        PresentationPageModel.Title = songTitle;
                        PresentationPageModel.Text = testLabel.Text;
                        PresentationPageModel.SongKey = songKey;
                        if (FontSize != 20)
                        {
                            PresentationPageModel.FontSize = FontSize;
                        }
                        break;

                    }
                }
                if (!String.IsNullOrEmpty(PresentationPageModel.Text) && !String.IsNullOrEmpty(PresentationPageModel.Title))
                {
                    return linesCount;
                }
                else
                {
                    return -1;
                }
            }

        }
    }
}

