using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Show_song_text.Droid.Persistence;
using Show_song_text.Database.Persistence;
using SQLite;
using Xamarin.Forms;
using Environment = System.Environment;

[assembly: Dependency(typeof(SQLiteDB))]
namespace Show_song_text.Droid.Persistence
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