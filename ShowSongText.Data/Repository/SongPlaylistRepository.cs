using System.Collections.Generic;
using System.Threading.Tasks;
using ShowSongText.Database.Abstraction;
using ShowSongText.Database.Models;
using SQLite;

namespace ShowSongText.Database.Repository
{
    interface ISongPlaylistRepository
    {
        Task<IEnumerable<SongPlaylist>> GetAllAsync();
        Task<SongPlaylist[]> GetAllArrayAsync();
        Task AddSongPlaylistRelation(SongPlaylist songPlaylistRelation);
    }

    public class SongPlaylistRepository : ISongPlaylistRepository
    {
        private SQLiteAsyncConnection _connection;
        public SongPlaylistRepository(ISQLiteDb db)
        {
            _connection = db.GetConnection();
            _connection.CreateTableAsync<Song>().Wait();
            _connection.CreateTableAsync<Position>().Wait();
            _connection.CreateTableAsync<SongPlaylist>().Wait();
            _connection.CreateTableAsync<SongPosition>().Wait();
        }

        public async Task<IEnumerable<SongPlaylist>> GetAllAsync()
        {
            return await _connection.Table<SongPlaylist>().ToListAsync();
        }

        public async Task<SongPlaylist[]> GetAllArrayAsync()
        {
            return await _connection.Table<SongPlaylist>().ToArrayAsync();
        }

        public async Task AddSongPlaylistRelation(SongPlaylist songPlaylistRelation)
        {
            await SQLiteNetExtensionsAsync.Extensions.WriteOperations.InsertWithChildrenAsync(_connection, songPlaylistRelation, false);
        }
    }
}
