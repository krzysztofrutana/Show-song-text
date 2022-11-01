using System.IO;
using ShowSongText.Droid.Persistence;
using SQLite;
using Xamarin.Forms;
using Environment = System.Environment;

[assembly: Dependency(typeof(SQLiteDB))]
namespace ShowSongText.Droid.Persistence
{
    public class SQLiteDB : ISQLiteDb
    {
        public SQLiteAsyncConnection GetConnection()
        {
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var path = Path.Combine(documentsPath, "Pomocnik_wokalisty.db3");
            return new SQLiteAsyncConnection(path);
        }
    }
}