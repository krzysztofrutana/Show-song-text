using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Show_song_text.Database.Models
{
    [Table("Position")]
    public class Position
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [ForeignKey(typeof(Playlist))]
        public int PlaylistId { get; set; }
        public int PositionOnPlaylist { get; set; }
    }
}
