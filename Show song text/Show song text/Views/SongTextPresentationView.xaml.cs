using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ShowSongText.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SongTextPresentationView : ContentPage
    {
        private static Label labelText;
        public SongTextPresentationView()
        {
            
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();
            labelText = ghostLabel;

        }

        public static Label GetGhostLabelInstance()
        {
            return labelText;
        }
    }
}