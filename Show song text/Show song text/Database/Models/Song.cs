using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Show_song_text.Database.Models
{
    public class Song
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [MaxLength(255)]
        public string Title { get; set; }
        [MaxLength(255)]
        public string Artist { get; set; }
        [MaxLength(255)]
        public string Text { get; set; }
        [MaxLength(255)]
        public string Chords { get; set; }
    }
}
