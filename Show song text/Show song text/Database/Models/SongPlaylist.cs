using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Show_song_text.Database.Models
{
    public class SongPlaylist
    {

        [ForeignKey(typeof(Song))]
        public int SongId { get; set; }

        [ForeignKey(typeof(Position))]
        public int PlaylistId { get; set; }
    }
}
