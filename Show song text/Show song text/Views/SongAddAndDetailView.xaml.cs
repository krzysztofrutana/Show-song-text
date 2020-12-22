using Show_song_text.Database.Persistence;
using Show_song_text.Database.Repository;
using Show_song_text.Database.ViewModels;
using Show_song_text.Interfaces;
using Show_song_text.Utils;
using Show_song_text.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace Show_song_text.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SongAddAndDetailView : ContentPage
    {
        public SongAddAndDetailView()
        {
            InitializeComponent();
        }
        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var list = (ListView)sender;
            list.SelectedItem = null;
        }
    }
}