using Show_song_text.Database.DOA;
using Show_song_text.Database.Models;
using Show_song_text.Database.Persistence;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Show_song_text.Database.Repository
{
    public class PositionRepository : IPositionDAO
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
