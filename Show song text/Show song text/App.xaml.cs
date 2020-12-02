using Show_song_text.Models;
using Show_song_text.Views;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Show_song_text
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();
            MainPage = new MainPageView();
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
