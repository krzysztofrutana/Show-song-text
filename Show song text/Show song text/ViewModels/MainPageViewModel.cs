using System;
using System.Collections.Generic;
using System.Reflection;
using ShowSongText.Abstraction;
using ShowSongText.Models;
using ShowSongText.Resources.Languages;
using ShowSongText.Utils;
using ShowSongText.Views;
using Xamarin.Forms;

namespace ShowSongText.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        #region Variables
        private readonly IPageService _pageService;
        #endregion


        #region Property
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
        #endregion

        #region Constructor
        public MainPageViewModel()
        {
            _pageService = new PageService();

            CreateMenuList();

        }
        #endregion


        #region Initialize
        private void CreateMenuList()
        {
            MenuListItems = new List<MasterMenuItem>
            {
                new MasterMenuItem() { Title = AppResources.SongList_Title, TargetType = typeof(SongListView), OptionIcon = "note.png" },
                new MasterMenuItem() { Title = AppResources.SongAddAndDetail_AddSong, TargetType = typeof(SongAddAndDetailView), OptionIcon = "plusRed.png" },
                new MasterMenuItem() { Title = AppResources.PlaylistList_Title, TargetType = typeof(PlaylistListView), OptionIcon = "playlist.png" },
                new MasterMenuItem() { Title = AppResources.ConnectionSettings_Title, TargetType = typeof(ConnectionSettingsView), OptionIcon = "connectionSettings.png" },
                new MasterMenuItem() { Title = AppResources.Settings_Title, TargetType = typeof(SettingsView), OptionIcon = "settings.png" },
                new MasterMenuItem() { Title = AppResources.RaportProblem_Title, TargetType = typeof(RaportProblemView), OptionIcon = "raport_problem.png" },
                new MasterMenuItem() { Title = AppResources.SupportMe_Title, TargetType = typeof(SupportMeView), OptionIcon = "support.png" }
            };
        }
        #endregion

        #region Property methods
        private async void OnMenuItemSelected(MasterMenuItem masterMenuItem)
        {
            try
            {
                if (masterMenuItem != null)
                {
                    if (masterMenuItem.Title.Equals(AppResources.SongAddAndDetail_AddSong))
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
                await _pageService.DisplayAlert(AppResources.AlertDialog_Error, e.InnerException.Message, AppResources.AlertDialog_OK);
            }
            catch (Exception e)
            {
                await _pageService.DisplayAlert(AppResources.AlertDialog_Error, e.Message, AppResources.AlertDialog_OK);
            }
        }
        #endregion

    }
}
