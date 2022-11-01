using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShowSongText.Database.Models
{
    public class SongPosition
    {

        [ForeignKey(typeof(Song))]
        public int SongId { get; set; }

        [ForeignKey(typeof(Position))]
        public int PositionId { get; set; }
    }
}
