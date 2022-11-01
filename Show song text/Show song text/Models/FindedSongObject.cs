using System;
using System.Collections.Generic;
using System.Text;

namespace ShowSongText.Models
{
    public class FindedSongObject
    {
        public string FullSongName { get; set; }
        public string Artist { get; set; }
        public string Title { get; set; }
        public string WorkingArtist { get; set; }
        public string WorkingTitle { get; set; }
        public string LinkToSong { get; set; }
        public string LinkToArtistSongs { get; set; }

    }
}
