using System;
using System.IO;
namespace Logger
{ public delegate void Message(string message);
    
    public class LogToFile : ILogger
    {
        public event Message Msg;
        private readonly string _path;
        public LogToFile(string path)
        {
            _path = path;
        }
        private void WriteToFile(string timeAndKind, string message)
        {
            try
            {
                using var file = new StreamWriter(_path, true);
                file.WriteLineAsync(timeAndKind + message);
            }
            catch (UnauthorizedAccessException)
            {
                Msg?.Invoke("Отказано в доступе");
                throw new Exception("Отказано в доступе");
            }
            catch (ArgumentException)
            {
                Msg?.Invoke("Параметр path пуст");
                throw new Exception("Параметр path пуст");
            }
            catch (DirectoryNotFoundException)
            {
                Msg?.Invoke("Указан недопустимый путь (например, он ведет на несопоставленный диск)");
                throw new Exception("Указан недопустимый путь (например, он ведет на несопоставленный диск)");
            }
            catch (PathTooLongException)
            {
                Msg?.Invoke(
                    "Указанный путь, имя файла или оба значения превышают максимальную длину, заданную в системе");
                throw new Exception(
                    "Указанный путь, имя файла или оба значения превышают максимальную длину, заданную в системе");
            }
            catch (IOException)
            {
                Msg?.Invoke(
                    "Параметр path включает неверный или недопустимый синтаксис имени файла, имени каталога или метки тома");
                throw new Exception(
                    "Параметр path включает неверный или недопустимый синтаксис имени файла, имени каталога или метки тома");
            }
            catch (ObjectDisposedException)
            {
                Msg?.Invoke("Удалено средство записи потока");
                throw new Exception("Удалено средство записи потока");
            }
            catch (InvalidOperationException)
            {
                Msg?.Invoke("Средство записи потока в настоящее время используется предыдущей операцией записи");
                throw new Exception(
                    "Средство записи потока в настоящее время используется предыдущей операцией записи");
            }
        }

        public void LogInfo(string message)
        {
            WriteToFile($"{DateTime.Now:u} [INFO]", message);
        }

        public void LogError(string message)
        {
            WriteToFile($"{DateTime.Now:u} [ERROR]", message);
        }

        public void LogWarning(string message)
        {
            WriteToFile($"{DateTime.Now:u} [WARNING]", message);
        }

        public void LogSuccess(string message)
        {
            WriteToFile($"{DateTime.Now:u} [SUCCESS]", message);
        }

        public void LogCustom(string type, string message)
        {
            WriteToFile($"{DateTime.Now:u} [{type}]", message);
        }

        public void Log(LogType type, string message)
        {
            WriteToFile($"{DateTime.Now:u} [{type.ToString()}]", message);
        }
    }
}