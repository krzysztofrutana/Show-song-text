using Show_song_text.ViewModels;
using Xamarin.Forms;
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

        private void Editor_Focused(object sender, FocusEventArgs e)
        {
            Editor editor = sender as Editor;
            var context = editor.BindingContext;
            var viewModel = BindingContext as SongAddAndDetailViewModel;
            viewModel.SetSelectedItemCommand.Execute(context);
        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var list = (ListView)sender;
            list.SelectedItem = null;
        }
    }
}