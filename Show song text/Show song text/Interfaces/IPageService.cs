using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Show_song_text.Interfaces
{
    public interface IPageService
    {
        void ChangePage(Page page);
        Task ChangePageAsync(Page page);
        Task<Page> PreviousDetailPage();
        Task<bool> DisplayAlert(string title, string message, string ok, string cancel);
        Task<string> DisplayEntry(string title, string message);
        Task DisplayAlert(string title, string message, string ok);
    }
}
