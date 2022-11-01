using System.Threading.Tasks;
using Xamarin.Forms;

namespace ShowSongText.Abstraction
{
    public interface IPageService
    {
        void ChangePage(Page page);
        Task ChangePageAsync(Page page);
        Task<Page> PreviousDetailPage();
        Task<bool> DisplayAlert(string title, string message, string ok, string cancel);
        Task<string> DisplayEntry(string title, string message);
        Task<string> DisplayPositionToChoose(string title, string concel, string destruction, string[] options);
        Task DisplayAlert(string title, string message, string ok);
    }
}
