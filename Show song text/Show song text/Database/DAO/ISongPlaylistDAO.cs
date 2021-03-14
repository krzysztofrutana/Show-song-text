using Show_song_text.Database.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Show_song_text.Database.DAO
{
    interface ISongPlaylistDAO
    {
        Task<IEnumerable<SongPlaylist>> GetAllAsync();
        Task<SongPlaylist[]> GetAllArrayAsync();
        Task AddSongPlaylistRelation(SongPlaylist songPlaylistRelation);
    }
}
