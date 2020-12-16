using Show_song_text.Database.Persistence;
using Show_song_text.Database.Repository;
using Show_song_text.Models;
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
    public partial class MainPageView : MasterDetailPage
    {

        public MainPageView()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();

        }
    }
    
}