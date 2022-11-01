using System.Collections.Generic;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace ShowSongText.Database.Models
{
    [Table("Song")]
    public class Song
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [ManyToMany(typeof(SongPlaylist))]
        public List<Playlist> Playlists { get; set; }
        [ManyToMany(typeof(SongPosition))]
        public List<Position> Positions { get; set; }
        [MaxLength(255)]
        public string Title { get; set; }
        [MaxLength(255)]
        public string Artist { get; set; }
        [MaxLength(255)]
        public string Text { get; set; }
        [MaxLength(255)]
        public string SongKey { get; set; }
        public string Chords { get; set; }
        public bool IsChecked { get; set; }
        public bool IsCheckBoxVisible { get; set; }
    }
}
