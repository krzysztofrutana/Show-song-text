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
            var songRepository = new SongRepository(DependencyService.Get<ISQLiteDb>());
            var pageService = new PageService();
            ViewModel = new MainPageViewModel(songRepository, pageService);
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();
        }
        private void OnMenuItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            //This method of start new page because I want have acces to menu button from page
            MasterMenuItem masterMenuItem = e.SelectedItem as MasterMenuItem;

            Detail = new NavigationPage((Page)Activator.CreateInstance(masterMenuItem.TargetType));
            IsPresented = false;

        }

        public MainPageViewModel ViewModel
        {
            get { return BindingContext as MainPageViewModel; }
            set { BindingContext = value; }
        }
    }
}