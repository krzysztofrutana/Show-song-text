using System;
using System.Collections.Generic;
using System.Text;

namespace Show_song_text.Utils
{
    public static class Events
    {
        public static string SongAdded = "AddSong";
        public static string SongUpdated = "UpdateSong";
        public static string SongDeleted = "DeleteSong";
        public static string SendSong = "SendSong";

        public static string SendPlaylist = "SendPlaylist";
        public static string PlaylistUpdated = "UpdatePlaylist";
        public static string PlaylistDeleted = "DeletePlaylist";

        public static string SendPlaylistToPresentation = "SendPlaylistToPresentation";
        public static string SendSongToPresentation = "SendSongToPresentation";

        public static string ServerIsRunning = "ServerIsRunning";
        public static string ClientConnected = "ClientConnected";
        public static string ClientDisconnected = "ClientDisconnected";

        public static string SendedText = "SendedText";

        public static string ConnectToServer = "ConnectToServer";

        public static string AddSongsToPlaylist = "AddSongsToPlaylist";



    }
}
