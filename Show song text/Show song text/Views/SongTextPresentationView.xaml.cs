using Show_song_text.Database.Models;
using Show_song_text.Database.Repository;
using Show_song_text.Interfaces;
using Show_song_text.Models;
using Show_song_text.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XamarinLabelFontSizer;

namespace Show_song_text.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SongTextPresentationView : ContentPage
    {
        private static Label label;
        public SongTextPresentationView()
        {
            
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();
            label = ghostLabel;

        }

        public static Label GetGhostLabelInstance()
        {
            return label;
        }
    }
}