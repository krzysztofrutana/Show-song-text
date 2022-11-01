using System;
using Genius;


namespace ShowSongText.Helpers
{
    public class GeniusAPIHelper
    {
        GeniusClient client { get; set; }

        public GeniusAPIHelper()
        {
            client = new GeniusClient(Settings.GeniusToken);
        }

        public async void Search(String text)
        {
            var search = await client.SongClient.GetSong(378195);
        }

    }
}
