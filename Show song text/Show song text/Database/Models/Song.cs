﻿using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Show_song_text.Database.Models
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
        public Boolean IsChecked { get; set; }
        public Boolean IsCheckBoxVisible { get; set; }
    }
}
