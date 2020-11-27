using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Show_song_text.Database.DOA;
using Show_song_text.Interfaces;
using Show_song_text.Models;
using Show_song_text.Views;
using Xamarin.Forms;

namespace Show_song_text.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        private readonly ISongDAO _songDAO;
        private readonly IPageService _pageService;

        public ICommand SelectMenuItemCommand { get; private set; }

        private List<MasterMenuItem> _MenuListItems;

        public List<MasterMenuItem> MenuListItems
        {
            get { return _MenuListItems; }
            set { _MenuListItems = value;
                OnPropertyChanged(nameof(MenuListItems));
            }
        }

        private MasterMenuItem _selectedItem;
        public MasterMenuItem SelectedItem { get { return _selectedItem; } set { _selectedItem = value; OnPropertyChanged(nameof(SelectedItem)); } }

        private Boolean _isPresent;

        public Boolean IsPresent
        {
            get { return _isPresent; }
            set { _isPresent = value;
                OnPropertyChanged(nameof(IsPresent));
            }
        }


        public string AppName { get; set; }



        public MainPageViewModel(ISongDAO songDAO, IPageService pageService)
        {
            _pageService = pageService;
            _songDAO = songDAO;

            SelectMenuItemCommand = new Command<MasterMenuItem>(async page => await SelectMenuItem(page));

            CreateMenuList();
            AppName = "Pomocnik wokalisty";
        }

        private async Task SelectMenuItem(MasterMenuItem masterMenuItem)
        {
            if(masterMenuItem == null)
            {
                return;
            }
            if(masterMenuItem.Title == "Lista utworów")
            {
                SelectedItem = null;
                IsPresent = false;
                return;
            }

            Type targetType = masterMenuItem.TargetType;

            var page = (Page)Activator.CreateInstance(targetType);

            await _pageService.PushAsync(page);
            
        }

        private void CreateMenuList()
        {
            MenuListItems = new List<MasterMenuItem>();
            MenuListItems.Add(new MasterMenuItem() { Title = "Lista utworów", TargetType = typeof(SongListView)});
            MenuListItems.Add(new MasterMenuItem() { Title = "Dodaj utwór", TargetType = typeof(SongDetailView) });
        }
        

    }
}
