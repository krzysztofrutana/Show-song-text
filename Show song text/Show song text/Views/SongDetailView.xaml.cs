using Show_song_text.Database.Persistence;
using Show_song_text.Database.Repository;
using Show_song_text.Database.ViewModels;
using Show_song_text.Interfaces;
using Show_song_text.Utils;
using Show_song_text.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace Show_song_text.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SongDetailView : ContentPage
    {
        public SongDetailView()
        {
            var songRepository = new SongRepository(DependencyService.Get<ISQLiteDb>());
            var pageService = new PageService();
            BindingContext = new SongDetailViewModel(songRepository, pageService);
            InitializeComponent();
        }
        public SongDetailView(SongViewModel songViewModel)
        {

             var songRepository = new SongRepository(DependencyService.Get<ISQLiteDb>());
             var pageService = new PageService();
             BindingContext = new SongDetailViewModel( songRepository, pageService, songViewModel ?? new SongViewModel());

            InitializeComponent();
        }
    }

}