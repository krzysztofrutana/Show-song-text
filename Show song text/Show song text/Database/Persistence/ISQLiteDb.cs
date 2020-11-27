using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Show_song_text.Database.Persistence
{
    public interface ISQLiteDb
    {
        SQLiteAsyncConnection GetConnection();
    }
}
