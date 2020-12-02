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
        public static List<Page> myNavigationStack;

        public MainPageView()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            myNavigationStack = new List<Page>();
            InitializeComponent();
        }

        //protected override bool OnBackButtonPressed()
        //{
        //    Page current = ((NavigationPage)((MasterDetailPage)Application.Current.MainPage).Detail).CurrentPage;
        //    var t = Detail.GetType();
        //    if (Detail.Title == "Lista utworów")
        //    {
        //        return base.OnBackButtonPressed();
        //    }
        //    else if (myNavigationStack != null)
        //    {
        //        Detail = new NavigationPage(myNavigationStack.LastOrDefault());
        //    }
        //    return true;
        //}
    }
    
}