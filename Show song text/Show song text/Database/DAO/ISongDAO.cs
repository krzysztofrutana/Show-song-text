using Show_song_text.Database.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Show_song_text.Database.DOA
{
    public interface ISongDAO
    {
        Task<IEnumerable<Song>> GetAllSongAsync();
        Task<IEnumerable<Song>> GetAllSongWithChildrenAsync();
        Task<Song> GetSong(int id);
        Task<Song> GetSongWithChildren(int id);
        Task AddSong(Song song);
        Task UpdateSong(Song song);
        Task DeleteSong(Song song);
    }
}
