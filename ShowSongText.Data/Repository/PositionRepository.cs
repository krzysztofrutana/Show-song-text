using System.Collections.Generic;
using System.Threading.Tasks;
using ShowSongText.Database.Abstraction;
using ShowSongText.Database.Models;
using SQLite;

namespace ShowSongText.Database.Repository
{
    public interface IPositionRepository
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
    public class PositionRepository : IPositionRepository
    {
        private SQLiteAsyncConnection _connection;
        public PositionRepository(ISQLiteDb db)
        {
            _connection = db.GetConnection();
            _connection.CreateTableAsync<Song>().Wait();
            _connection.CreateTableAsync<Position>().Wait();
            _connection.CreateTableAsync<SongPlaylist>().Wait();
            _connection.CreateTableAsync<SongPosition>().Wait();
        }

        public async Task AddPosition(Position position)
        {
            await SQLiteNetExtensionsAsync.Extensions.WriteOperations.InsertWithChildrenAsync(_connection, position, false);
        }

        public async Task DeletePosition(Position position)
        {
            await SQLiteNetExtensionsAsync.Extensions.WriteOperations.DeleteAsync(_connection, position, false);
        }

        public async Task<IEnumerable<Position>> GetAllPositionAsync()
        {
            return await _connection.Table<Position>().ToListAsync();
        }

        public async Task<Position[]> GetAllPositionArrayAsync()
        {
            return await _connection.Table<Position>().ToArrayAsync();
        }

        public async Task<IEnumerable<Position>> GetAllPositionWithChildrenAsync()
        {
            return await SQLiteNetExtensionsAsync.Extensions.ReadOperations.GetAllWithChildrenAsync<Position>(_connection);
        }

        public async Task<Position> GetPosition(int id)
        {
            return await _connection.FindAsync<Position>(id);
        }

        public async Task<Position> GetPositionWithChildren(int id)
        {
            return await SQLiteNetExtensionsAsync.Extensions.ReadOperations.GetWithChildrenAsync<Position>(_connection, id, true);
        }

        public async Task UpdatePosition(Position position)
        {
            await SQLiteNetExtensionsAsync.Extensions.WriteOperations.UpdateWithChildrenAsync(_connection, position);
        }

    }
}
