using System.Collections.Generic;
using System.Threading.Tasks;
using ShowSongText.Database.Abstraction;
using ShowSongText.Database.Models;
using SQLite;

namespace ShowSongText.Database.Repository
{
    interface ISongPositionRepository
    {
        Task<IEnumerable<SongPosition>> GetAllAsync();
        Task<SongPosition[]> GetAllArrayAsync();
        Task AddSongPositionRelation(SongPosition songPositionRelation);
    }
    public class SongPositionRepository : ISongPositionRepository
    {
        private readonly SQLiteAsyncConnection _connection;
        public SongPositionRepository(ISQLiteDb db)
        {
            _connection = db.GetConnection();
            _connection.CreateTableAsync<Song>().Wait();
            _connection.CreateTableAsync<Position>().Wait();
            _connection.CreateTableAsync<SongPlaylist>().Wait();
            _connection.CreateTableAsync<SongPosition>().Wait();
        }

        public async Task<IEnumerable<SongPosition>> GetAllAsync()
        {
            return await _connection.Table<SongPosition>().ToListAsync();
        }

        public async Task<SongPosition[]> GetAllArrayAsync()
        {
            return await _connection.Table<SongPosition>().ToArrayAsync();
        }

        public async Task AddSongPositionRelation(SongPosition songPositionRelation)
        {
            await SQLiteNetExtensionsAsync.Extensions.WriteOperations.InsertWithChildrenAsync(_connection, songPositionRelation, false);
        }
    }
}
