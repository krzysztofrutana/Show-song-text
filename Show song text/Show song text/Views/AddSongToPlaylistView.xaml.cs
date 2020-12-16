using Show_song_text.Database.ViewModels;
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
    public partial class AddSongToPlaylistView : ContentPage
    {
        public AddSongToPlaylistView()
        {
            InitializeComponent();
        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var list = (ListView)sender;
            list.SelectedItem = null;
        }

        private void CheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            var viewModel = BindingContext as AddSongToPlaylistViewModel;
            CheckBox checkbox = (CheckBox)sender;
            SongViewModel selectedSong = checkbox.BindingContext as SongViewModel;
            if (e.Value == true)
            {
                viewModel.SelectSongCommand.Execute(selectedSong);
            }
            else if (e.Value == false)
            {
                viewModel.UnselectSongCommand.Execute(selectedSong);
            }
        }
    }
}