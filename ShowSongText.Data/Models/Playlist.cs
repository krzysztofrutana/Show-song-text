using System.Collections.Generic;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace ShowSongText.Database.Models
{
    [Table("Playlist")]
    public class Playlist
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [MaxLength(255)]
        public string Name { get; set; }
        [ManyToMany(typeof(SongPlaylist))]
        public List<Song> Songs { get; set; }
        public bool CustomSongsOrder { get; set; }
    }
}
