using Show_song_text.Database.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Show_song_text.Database.DOA
{
    public interface IPositionDAO
    {
        Task<IEnumerable<Position>> GetAllPositionAsync();
        Task<Position[]> GetAllPositionArrayAsync();
        Task<IEnumerable<Position>> GetAllPositionWithChildrenAsync();
        Task<Position> GetPosition(int id);
        Task<Position> GetPositionWithChildren(int id);
        Task AddPosition(Position position);
        Task UpdatePosition(Position position);
        Task DeletePosition(Position position);
    }
}
