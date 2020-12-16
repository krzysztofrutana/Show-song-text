using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Show_song_text.Database.Models
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
        public Boolean CustomSongsOrder { get; set; }
    }
}
