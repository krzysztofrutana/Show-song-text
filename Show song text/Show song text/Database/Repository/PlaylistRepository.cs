using Show_song_text.Database.DAO;
using Show_song_text.Database.Models;
using Show_song_text.Database.Persistence;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Show_song_text.Database.Repository
{
    public class PlaylistRepository : IPlaylistDAO
    {
        private readonly SQLiteAsyncConnection _connection;
        public PlaylistRepository(ISQLiteDb db)
        {
            _connection = db.GetConnection();
            CreateTable();
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

        public async Task<IEnumerable<Playlist>> GetaAllPlaylistWithChildrenAsync()
        {
            var playlists = await GetAllPlaylistAsync();
            foreach(Playlist playlist in playlists)
            {
                var query = await _connection.QueryAsync<Song>("select * from Song where PlaylistID = ?", playlist.Id);
                playlist.Songs = query;
            }

            return playlists;
        }

        public async Task<Playlist> GetPlaylist(int id)
        {
            return await _connection.FindAsync<Playlist>(id);
        }

        public async Task<Playlist> GetPlaylistWithSongs(int id)
        {
            Playlist playlist = await _connection.FindAsync<Playlist>(id);
            var query = await _connection.QueryAsync<Song>("select * from Song where PlaylistID = ?", playlist.Id);
            playlist.Songs = query;
            return playlist;
        }

        public async Task UpdatePlaylist(Playlist playlist)
        {
            await _connection.UpdateAsync(playlist);
        }

        private async void CreateTable()
        {
            await _connection.CreateTableAsync<Playlist>();
        }
    }
}
