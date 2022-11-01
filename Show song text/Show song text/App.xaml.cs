using ShowSongText.Helpers;
using ShowSongText.Models;
using ShowSongText.Resources.Languages;
using ShowSongText.Views;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ShowSongText
{
    public partial class App : Application
    {
        public static int ScreenHeight { get; set; }
        public static int ScreenWidth { get; set; }

        public static string DocumentsFolderPath { get; set; }

        public App()
        {
            if (!Settings.ChoosenLanguage.Equals("none"))
            {
                CultureInfo language = new CultureInfo(Settings.ChoosenLanguage);
                Thread.CurrentThread.CurrentUICulture = language;
                AppResources.Culture = language;
            }
            InitializeComponent();
            MainPage = new MainPageView();
            //Background color
            MainPage.SetValue(NavigationPage.BarBackgroundColorProperty, Color.Black);

            //Title color
            MainPage.SetValue(NavigationPage.BarTextColorProperty, Color.White);
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
