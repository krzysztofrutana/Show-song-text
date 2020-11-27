using System;
using System.Collections.Generic;
using System.Text;

namespace Show_song_text.Utils
{
    public static class Events
    {
        public static string SongAdded = "AddSong";
        public static string SongUpdated = "UpdateSong";
        public static string ChangedArtist = "ArtistChange";
        public static string ChangedTitle = "TitleChange";
        public static string ChangedText = "TextChange";
        public static string ChangedChords = "ChordsChange";
    }
}
