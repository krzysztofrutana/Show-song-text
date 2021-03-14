using Show_song_text.Database.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Show_song_text.Database.DAO
{
    interface ISongPositionDAO
    {
        Task<IEnumerable<SongPosition>> GetAllAsync();
        Task<SongPosition[]> GetAllArrayAsync();
        Task AddSongPositionRelation(SongPosition songPositionRelation);
    }
}
