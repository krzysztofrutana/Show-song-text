using ShowSongText.ViewModels;
using ShowSongText.ViewModels.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ShowSongText.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PlaylistDetailView : ContentPage
    {
        public PlaylistDetailView()
        {
            InitializeComponent();
        }

        private void CheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            var viewModel = BindingContext as PlaylistDetailViewModel;
            CheckBox checkbox = (CheckBox)sender;
            SongViewModel selectedSong = checkbox.BindingContext as SongViewModel;
            if (e.Value == true)
            {
                viewModel.SelectAsToDeleteCommand.Execute(selectedSong);
            }
            else if (e.Value == false)
            {
                viewModel.UnselectAsToDeleteCommand.Execute(selectedSong);
            }
        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var list = (ListView)sender;
            list.SelectedItem = null;
        }
    }
}