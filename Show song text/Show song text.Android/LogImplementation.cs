using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Util;
using Show_song_text.Interfaces;
using Show_song_text.Droid;

[assembly: Xamarin.Forms.Dependency(typeof(LogImplementation))]
namespace Show_song_text.Droid
{
    public class LogImplementation : ILogInterface
    {
        public void Debug(string TAG, string message)
        {
            Log.Debug(TAG, message);
        }

        public void Error(string TAG, string message)
        {
            Log.Error(TAG, message);
        }

        public void Info(string TAG, string message)
        {
            Log.Info(TAG, message);
        }

        public void Verbose(string TAG, string message)
        {
            Log.Verbose(TAG, message);
        }

        public void Warn(string TAG, string message)
        {
            Log.Warn(TAG, message);
        }
    }
}