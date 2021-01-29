using System;
using System.Collections.Generic;
using System.Text;

namespace Show_song_text.Interfaces
{
    public interface ILogInterface
    {
        void Verbose(string TAG, string message);
        void Info(string TAG, string message);
        void Debug(string TAG, string message);
        void Error(string TAG, string message);
        void Warn(string TAG, string message);
    }
}
