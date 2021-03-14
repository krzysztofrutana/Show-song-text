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
    public class SongPositionRepository : ISongPositionDAO
    {
        private SQLiteAsyncConnection _connection;
        public SongPositionRepository(ISQLiteDb db)
        {
            _connection = db.GetConnection();
            _connection.CreateTableAsync<Song>().Wait();
            _connection.CreateTableAsync<Position>().Wait();
            _connection.CreateTableAsync<SongPlaylist>().Wait();
            _connection.CreateTableAsync<SongPosition>().Wait();

            //bool update =  CheckIfUpdateIsNeeded();
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
