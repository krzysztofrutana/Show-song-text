using Show_song_text.Database.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Show_song_text.Models.DatabaseBackup
{
    [XmlRoot("Database", Namespace = "http://www.krzysztofrutana.pl",
   IsNullable = false)]
    public class DatabaseModel
    {
        [XmlArray("Playlists")]
        public Playlist[] Playlists { get; set; }
        [XmlArray("Positions")]
        public Position[] Positions { get; set; }
        [XmlArray("Songs")]
        public Song[] Songs { get; set; }
        [XmlArray("SongPlaylists")]
        public SongPlaylist[] SongPlaylists { get; set; }
        [XmlArray("SongPositions")]
        public SongPosition[] SongPositions { get; set; }
    }
}
