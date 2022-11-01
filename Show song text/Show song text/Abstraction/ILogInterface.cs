namespace ShowSongText.Abstraction
{
    public interface ILogInterface
    {
        void Verbose(string TAG, string message);
        void Info(string TAG, string message);
        void Debug(string TAG, string message);
        void Error(string TAG, string message);
        void Warn(string TAG, string message);
    }
}
