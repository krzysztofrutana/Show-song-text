using Show_song_text.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using XamarinLabelFontSizer;

namespace Show_song_text.Utils
{
    public static class PresentationPageHelper
    {

        private static double FontSize { get; set; }
        public static PresentationPageModel presentationPageModel { get; set; }

        public static Boolean CheckIfFit(Label label, double fontSize)
        {
            FontCalc lowerFontCalc = new FontCalc(label, fontSize, App.ScreenWidth * 0.9, App.ScreenHeight - 120);
            if (lowerFontCalc.TextHeight > App.ScreenHeight - 120)
            {
                return false;
            }
            else
            {
                {
                    return true;
                }

            }
        }

        public static int GetFitPageModel(string[] textToFit, Label testLabel, string songTitle, Double fontSize)
        {

            FontSize = fontSize;

            int songTextLines = textToFit.Length;

            int linesCount = songTextLines;
            presentationPageModel = new PresentationPageModel();
            for (int i = songTextLines - 1; i >= 0; i--)
            {
                string[] temp = new string[i + 1];
                for (int j = 0; j <= i; j++)
                {
                    temp[j] = textToFit[j];
                    temp[j] = temp[j].Trim();

                }
                testLabel.Text = string.Join(Environment.NewLine.ToString(), temp);
                Boolean isFit = PresentationPageHelper.CheckIfFit(testLabel, FontSize);
                if (isFit)
                {
                    linesCount = i;
                    presentationPageModel.Title = songTitle;
                    presentationPageModel.Text = testLabel.Text;
                    if (FontSize != 20)
                    {
                        presentationPageModel.FontSize = FontSize;
                    }
                    break;

                }
            }
            if (!String.IsNullOrEmpty(presentationPageModel.Text) && !String.IsNullOrEmpty(presentationPageModel.Title))
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
