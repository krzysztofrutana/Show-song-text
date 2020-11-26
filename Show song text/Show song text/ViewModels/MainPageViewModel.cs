using System;
using System.Collections.Generic;
using System.Text;
using Show_song_text.Models;
using Show_song_text.Views;
using Xamarin.Forms;

namespace Show_song_text.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        private List<MasterMenuItem> _MenuListItems;

        public List<MasterMenuItem> MenuListItems
        {
            get { return _MenuListItems; }
            set { _MenuListItems = value;
                OnPropertyChanged(nameof(MenuListItems));
            }
        }

        public string AppName { get; set; }



        public MainPageViewModel()
        {
            CreateMenuList();
            AppName = "Pomocnik wokalisty";
        }

        private void CreateMenuList()
        {
            MenuListItems = new List<MasterMenuItem>();
            MenuListItems.Add(new MasterMenuItem() { Title = "Lista utwórów", TargetType = typeof(SongListView)});
            MenuListItems.Add(new MasterMenuItem() { Title = "Dodaj utwór", TargetType = typeof(AddNewSong) });
        }
        

    }
}
