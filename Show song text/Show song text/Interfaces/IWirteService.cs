using Show_song_text.Models.DatabaseBackup;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Show_song_text.Interfaces
{
    public interface IWirteService
    {
        void wirteFile(string fileName, string json);
    }
}
