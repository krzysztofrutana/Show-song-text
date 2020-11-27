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
    public class SongRepository : ISongDAO
    {
        private SQLiteAsyncConnection _connection;
        public SongRepository(ISQLiteDb db)
        {
            _connection = db.GetConnection();
            _connection.CreateTableAsync<Song>();
        }
        public async Task AddSong(Song song)
        {
            await _connection.InsertAsync(song);
        }

        public async Task DeleteSong(Song song)
        {
            await _connection.DeleteAsync(song);
        }

        public async Task<IEnumerable<Song>> GetAllSongAsync()
        {
            return await _connection.Table<Song>().ToListAsync();
        }

        public async Task<Song> GetSong(int id)
        {
            return await _connection.FindAsync<Song>(id); 
        }

        public async Task UpdateSong(Song song)
        {
            await _connection.UpdateAsync(song);
        }
    }
}
