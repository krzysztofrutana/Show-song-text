using Show_song_text.Database.Persistence;
using Show_song_text.Database.Repository;
using Show_song_text.Database.ViewModels;
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
            InitializeComponent();
        }

        private void CheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            var viewModel = BindingContext as SongListViewModel;
            CheckBox checkbox = (CheckBox)sender;
            SongViewModel selectedSong = checkbox.BindingContext as SongViewModel;
            if (e.Value == true)
            {
                viewModel.AddToPlaylistCommand.Execute(selectedSong);
            } else if (e.Value == false)
            {
                viewModel.DeleteFromPlaylistCommand.Execute(selectedSong);
            }
        }
    }
}