using Android.Util;
using ShowSongText.Abstraction;
using ShowSongText.Droid;

[assembly: Xamarin.Forms.Dependency(typeof(LogImplementation))]
namespace ShowSongText.Droid
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