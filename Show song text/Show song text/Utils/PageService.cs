using Show_song_text.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Show_song_text.Utils
{
    public class PageService : IPageService
    {
        public async Task DisplayAlert(string title, string message, string ok)
        {
            await MainPage.DisplayAlert(title, message, ok);
        }

        public async Task<bool> DisplayAlert(string title, string message, string ok, string cancel)
        {
            return await MainPage.DisplayAlert(title, message, ok, cancel);
        }

        public void ChangePage(Page page)
        {
            
            MainPage.Detail = new NavigationPage(page);
            MainPage.IsPresented = false;
        }

        public async Task ChangePageAsync(Page page)
        {
            await DetailPage.Navigation.PushAsync(page);
            MainPage.IsPresented = false;
        }

        public async Task<Page> PreviousDetailPage()
        {
            return await MainPage.Navigation.PopAsync();
        }

        private MasterDetailPage MainPage
        {
            get { return (Application.Current.MainPage as MasterDetailPage); }
        }

        private NavigationPage DetailPage
        {
            get { return (NavigationPage)((MasterDetailPage)Application.Current.MainPage).Detail; }
        }
    }
}
