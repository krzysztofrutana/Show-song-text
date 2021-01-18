using Xamarin.Forms;
using Xamarin.Forms.Xaml;

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