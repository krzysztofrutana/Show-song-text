using Show_song_text.Database.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Show_song_text.Database.DAO
{
    public interface IPlaylistDAO
    {
        Task<IEnumerable<Playlist>> GetAllPlaylistAsync();
        Task<IEnumerable<Playlist>> GetaAllPlaylistWithChildrenAsync();
        Task<Playlist> GetPlaylist(int id);
        Task<Playlist> GetPlaylistWithSongs(int id);
        Task AddPlaylist(Playlist playlist);
        Task UpdatePlaylist(Playlist playlist);
        Task DeletePlaylist(Playlist playlist);
    }
}
