﻿using System.Threading.Tasks;
using ShowSongText.Abstraction;
using Xamarin.Forms;

namespace ShowSongText.Utils
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

        public async Task<string> DisplayEntry(string title, string message)
        {
            return await MainPage.DisplayPromptAsync(title, message);
        }

        public async Task<string> DisplayPositionToChoose(string title, string concel, string destruction, string[] options)
        {
            return await MainPage.DisplayActionSheet(title, concel, destruction, options);
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
            return await DetailPage.Navigation.PopAsync();
        }

        private FlyoutPage MainPage
        {
            get { return (Application.Current.MainPage as FlyoutPage); }
        }

        private NavigationPage DetailPage
        {
            get { return (NavigationPage)((FlyoutPage)Application.Current.MainPage).Detail; }
        }
    }
}
