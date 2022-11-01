using ShowSongText.ViewModels;
using ShowSongText.ViewModels.DTO;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ShowSongText.Views
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
            }
            else if (e.Value == false)
            {
                viewModel.DeleteFromPlaylistCommand.Execute(selectedSong);
            }
        }

    }
}