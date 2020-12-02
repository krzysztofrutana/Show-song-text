using Show_song_text.Database.DAO;
using Show_song_text.Database.Models;
using Show_song_text.Database.Persistence;
using SQLite;
using SQLiteNetExtensionsAsync;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Show_song_text.Database.Repository
{
    public class PlaylistRepository : IPlaylistDAO
    {
        private SQLiteAsyncConnection _connection;
        public PlaylistRepository(ISQLiteDb db)
        {
            _connection = db.GetConnection();
            _connection.CreateTableAsync<Playlist>();
        }
        public async Task AddPlaylist(Playlist playlist)
        {
            await _connection.InsertAsync(playlist);
        }

        public async Task DeletePlaylist(Playlist playlist)
        {
            await _connection.DeleteAsync(playlist);
        }

        public async Task<IEnumerable<Playlist>> GetAllPlaylistAsync()
        {
            return await _connection.Table<Playlist>().ToListAsync();
        }

        public async Task<Playlist> GetPlaylist(int id)
        {
            return await _connection.FindAsync<Playlist>(id);
        }

        public async Task UpdatePlaylist(Playlist playlist)
        {
            await _connection.UpdateAsync(playlist);
        }
    }
}
