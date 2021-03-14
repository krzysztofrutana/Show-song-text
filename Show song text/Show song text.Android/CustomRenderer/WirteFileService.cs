using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using Show_song_text.Droid.CustomRenderer;
using Show_song_text.Interfaces;
using Show_song_text.Models.DatabaseBackup;

[assembly: Xamarin.Forms.Dependency(typeof(WirteFileService))]
namespace Show_song_text.Droid.CustomRenderer
{
    public class WirteFileService : IWirteService
    {
        void IWirteService.wirteFile(string fileName, string json)
        {

            string path = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads).AbsolutePath;

            string fileFullPath = Path.Combine(path, fileName);

            File.WriteAllText(fileFullPath, json);
        }
    }
}