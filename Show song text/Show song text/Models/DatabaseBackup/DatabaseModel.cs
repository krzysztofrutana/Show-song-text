using System.Xml.Serialization;
using ShowSongText.Database.Models;

namespace ShowSongText.Models.DatabaseBackup
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
