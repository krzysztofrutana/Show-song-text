using System.IO;
using ShowSongText.Abstraction;
using ShowSongText.Droid.CustomRenderer;

[assembly: Xamarin.Forms.Dependency(typeof(WirteFileService))]
namespace ShowSongText.Droid.CustomRenderer
{
    public class WirteFileService : IWirteService
    {
        void IWirteService.WirteFile(string fileName, string json)
        {

            string path = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads).AbsolutePath;

            string fileFullPath = Path.Combine(path, fileName);

            File.WriteAllText(fileFullPath, json);
        }
    }
}