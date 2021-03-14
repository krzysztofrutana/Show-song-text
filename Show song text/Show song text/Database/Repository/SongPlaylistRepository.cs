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
    public class SongPlaylistRepository : ISongPlaylistDAO
    {
        private SQLiteAsyncConnection _connection;
        public SongPlaylistRepository(ISQLiteDb db)
        {
            _connection = db.GetConnection();
            _connection.CreateTableAsync<Song>().Wait();
            _connection.CreateTableAsync<Position>().Wait();
            _connection.CreateTableAsync<SongPlaylist>().Wait();
            _connection.CreateTableAsync<SongPosition>().Wait();

            //bool update =  CheckIfUpdateIsNeeded();
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
