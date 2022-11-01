using SQLite;

namespace ShowSongText.Database.Abstraction
{
    public interface ISQLiteDb
    {
        SQLiteAsyncConnection GetConnection();
    }
}
