using System.Collections.Generic;
using System.Threading.Tasks;
using ShowSongText.Database.Abstraction;
using ShowSongText.Database.Models;
using SQLite;

namespace ShowSongText.Database.Repository
{
    public interface ISongRepository
    {
        Task<IEnumerable<Song>> GetAllSongAsync();
        Task<Song[]> GetAllSongArrayAsync();
        Task<IEnumerable<Song>> GetAllSongWithChildrenAsync();
        Task<Song> GetSong(int id);
        Task<Song> GetSongWithChildren(int id);
        Task AddSong(Song song);
        Task UpdateSong(Song song);
        Task DeleteSong(Song song);
    }
    public class SongRepository : ISongRepository
    {
        private SQLiteAsyncConnection _connection;
        public SongRepository(ISQLiteDb db)
        {
            _connection = db.GetConnection();
            _connection.CreateTableAsync<Song>().Wait();
            _connection.CreateTableAsync<Position>().Wait();
            _connection.CreateTableAsync<SongPlaylist>().Wait();
            _connection.CreateTableAsync<SongPosition>().Wait();
        }
        public async Task AddSong(Song song)
        {
            await SQLiteNetExtensionsAsync.Extensions.WriteOperations.InsertWithChildrenAsync(_connection, song, false);
        }

        public async Task DeleteSong(Song song)
        {
            await SQLiteNetExtensionsAsync.Extensions.WriteOperations.DeleteAsync(_connection, song, false);
        }

        public async Task<IEnumerable<Song>> GetAllSongAsync()
        {
            return await _connection.Table<Song>().ToListAsync();
        }

        public async Task<Song[]> GetAllSongArrayAsync()
        {
            return await _connection.Table<Song>().ToArrayAsync();
        }

        public async Task<IEnumerable<Song>> GetAllSongWithChildrenAsync()
        {
            return await SQLiteNetExtensionsAsync.Extensions.ReadOperations.GetAllWithChildrenAsync<Song>(_connection);
        }

        public async Task<Song> GetSong(int id)
        {
            return await _connection.FindAsync<Song>(id);
        }

        public async Task<Song> GetSongWithChildren(int id)
        {
            return await SQLiteNetExtensionsAsync.Extensions.ReadOperations.GetWithChildrenAsync<Song>(_connection, id, true);
        }

        public async Task UpdateSong(Song song)
        {
            await SQLiteNetExtensionsAsync.Extensions.WriteOperations.UpdateWithChildrenAsync(_connection, song);
        }
    }
}
