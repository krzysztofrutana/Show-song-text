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
        [OneToMany]
        public List<Song> Songs { get; set; }
    }
}
