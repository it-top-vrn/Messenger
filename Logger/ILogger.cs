namespace Logger
{
    public interface ILogger
    {
        public void LogInfo(string message)
        {
        }

        public void LogError(string message)
        {
        }

        public void LogWarning(string message)
        {
        }

        public void LogSuccess(string message)
        {
        }

        public void LogCustom(string type, string message)
        {
        }

        public void Log(LogType type, string message)
        {
        }
    }
}