using Show_song_text.Database.Persistence;
using Show_song_text.Database.Repository;
using Show_song_text.Utils;
using Show_song_text.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Show_song_text.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SongListView : ContentPage
    {
        public SongListView()
        {
            var songRepository = new SongRepository(DependencyService.Get<ISQLiteDb>());
            var pageService = new PageService();
            ViewModel = new SongListViewModel(songRepository, pageService);
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            ViewModel.LoadSongsCommand.Execute(null);
            base.OnAppearing();
        }

        private void OnSongSelected(object sender, SelectedItemChangedEventArgs e)
        {
            ViewModel.SelectSongCommand.Execute(e.SelectedItem);
        }
        public SongListViewModel ViewModel
        {
            get { return BindingContext as SongListViewModel; }
            set { BindingContext = value; }
        }
    }
}