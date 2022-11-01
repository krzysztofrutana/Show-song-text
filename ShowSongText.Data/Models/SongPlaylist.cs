using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShowSongText.Database.Models
{
    public class SongPlaylist
    {

        [ForeignKey(typeof(Song))]
        public int SongId { get; set; }

        [ForeignKey(typeof(Position))]
        public int PlaylistId { get; set; }
    }
}
