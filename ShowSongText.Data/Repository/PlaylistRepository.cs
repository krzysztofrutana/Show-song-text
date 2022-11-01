using System.Collections.Generic;
using System.Threading.Tasks;
using ShowSongText.Database.Abstraction;
using ShowSongText.Database.Models;
using SQLite;

namespace ShowSongText.Database.Repository
{
    public interface IPlaylistRepository
    {
        Task<IEnumerable<Playlist>> GetAllPlaylistAsync();
        Task<Playlist[]> GetAllPlaylistArrayAsync();
        Task<IEnumerable<Playlist>> GetaAllPlaylistWithChildrenAsync();
        Task<Playlist> GetPlaylist(int id);
        Task<Playlist> GetPlaylistWithSongs(int id);
        Task AddPlaylist(Playlist playlist);
        Task UpdatePlaylist(Playlist playlist);
        Task DeletePlaylist(Playlist playlist);
    }

    public class PlaylistRepository : IPlaylistRepository
    {
        private readonly SQLiteAsyncConnection _connection;
        public PlaylistRepository(ISQLiteDb db)
        {
            _connection = db.GetConnection();
            _connection.CreateTableAsync<Playlist>().Wait();
            _connection.CreateTableAsync<Position>().Wait();
            _connection.CreateTableAsync<SongPlaylist>().Wait();
            _connection.CreateTableAsync<SongPosition>().Wait();
        }
        public async Task AddPlaylist(Playlist playlist)
        {
            await SQLiteNetExtensionsAsync.Extensions.WriteOperations.InsertWithChildrenAsync(_connection, playlist, false);
        }

        public async Task DeletePlaylist(Playlist playlist)
        {
            await SQLiteNetExtensionsAsync.Extensions.WriteOperations.DeleteAsync(_connection, playlist, false);
        }

        public async Task<IEnumerable<Playlist>> GetAllPlaylistAsync()
        {
            return await _connection.Table<Playlist>().ToListAsync();
        }

        public async Task<Playlist[]> GetAllPlaylistArrayAsync()
        {
            return await _connection.Table<Playlist>().ToArrayAsync();
        }

        public async Task<IEnumerable<Playlist>> GetaAllPlaylistWithChildrenAsync()
        {
            return await SQLiteNetExtensionsAsync.Extensions.ReadOperations.GetAllWithChildrenAsync<Playlist>(_connection, null, true);
        }

        public async Task<Playlist> GetPlaylist(int id)
        {
            return await _connection.FindAsync<Playlist>(id);
        }

        public async Task<Playlist> GetPlaylistWithSongs(int id)
        {
            return await SQLiteNetExtensionsAsync.Extensions.ReadOperations.GetWithChildrenAsync<Playlist>(_connection, id, true);

        }

        public async Task UpdatePlaylist(Playlist playlist)
        {
            await SQLiteNetExtensionsAsync.Extensions.WriteOperations.UpdateWithChildrenAsync(_connection, playlist);
        }
    }
}
