using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Show_song_text.Database.DOA;
using Show_song_text.Database.Persistence;
using Show_song_text.Database.Repository;
using Show_song_text.Interfaces;
using Show_song_text.Models;
using Show_song_text.PresentationServerUtilis;
using Show_song_text.Utils;
using Show_song_text.Views;
using Xamarin.Forms;

namespace Show_song_text.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        // VARIABLE START

        private readonly IPageService _pageService;

        // VARIABLE END


        // PROPERTY START
        private List<MasterMenuItem> _MenuListItems;

        public List<MasterMenuItem> MenuListItems
        {
            get { return _MenuListItems; }
            set
            {
                _MenuListItems = value;
                OnPropertyChanged(nameof(MenuListItems));
            }
        }

        private MasterMenuItem _selectedItem;
        public MasterMenuItem SelectedItem { get { return _selectedItem; } set { _selectedItem = value; OnPropertyChanged(nameof(SelectedItem)); OnMenuItemSelected(value); } }

        private Boolean _isPresent;

        public Boolean IsPresent
        {
            get { return _isPresent; }
            set
            {
                _isPresent = value;
                OnPropertyChanged(nameof(IsPresent));
            }
        }

        public string AppName { get; set; }
        // PROPERTY END

        // CONSTRUCTOR START
        public MainPageViewModel()
        {
            _pageService = new PageService();

            CreateMenuList();
            AppName = "Pomocnik wokalisty";

        }
        // CONSTRUCTOR END


        // INITIALIZE METHOD START
        private void CreateMenuList()
        {
            MenuListItems = new List<MasterMenuItem>();
            MenuListItems.Add(new MasterMenuItem() { Title = "Lista utworów", TargetType = typeof(SongListView), OptionIcon="note.png" });
            MenuListItems.Add(new MasterMenuItem() { Title = "Dodaj utwór", TargetType = typeof(SongAddAndDetailView), OptionIcon = "plusRed.png" });
            MenuListItems.Add(new MasterMenuItem() { Title = "Listy odtwarzania", TargetType = typeof(PlaylistListView), OptionIcon = "playlist.png" });
            MenuListItems.Add(new MasterMenuItem() { Title = "Ustawienia połączeń", TargetType = typeof(ConnectionSettingsView), OptionIcon = "connectionSettings.png" });
            MenuListItems.Add(new MasterMenuItem() { Title = "Ustawienia", TargetType = typeof(SettingsView), OptionIcon = "settings.png" });
        }
        // INITIALIZE METHOD END

        // PROPERTY METHOD START

        private async void OnMenuItemSelected(MasterMenuItem masterMenuItem)
        {
            try
            {
                if (masterMenuItem != null)
                {
                    if (masterMenuItem.Title == "Dodaj utwór")
                    {
                        Page page = (Page)Activator.CreateInstance(masterMenuItem.TargetType);
                        await _pageService.ChangePageAsync(page);
                        SelectedItem = null;
                    }
                    else
                    {
                        Page page = (Page)Activator.CreateInstance(masterMenuItem.TargetType);
                        _pageService.ChangePage(page);
                        SelectedItem = null;
                    }
                }
            }
            catch (TargetInvocationException e)
            {
                await _pageService.DisplayAlert("Błąd", e.InnerException.Message, "OK");
            }
            catch (Exception e)
            {
                await _pageService.DisplayAlert("Błąd", e.Message, "OK");
            }
        }


        // PROPERTY METHOD END

    }
}
