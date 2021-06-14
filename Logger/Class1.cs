using System;
using System.IO;
using System.Threading.Tasks;

namespace Logger
{
    public class LogToFile
    {
        private readonly string _path;

        public LogToFile(string path)
        {
            _path = path;
        }

        public enum LogType
        {
            Info, Success, Error, Warning
        }
        private async Task WriteToFile(string message)
        {
            try
            {
                await using var file = new StreamWriter(_path, true);
                await file.WriteLineAsync(message);
            }
            

            catch (UnauthorizedAccessException)
            {
                throw new Exception("Отказано в доступе");
            }

            catch (ArgumentException)
            {
                throw new Exception("Параметр path пуст");
            }

            catch (DirectoryNotFoundException)
            {
                throw new Exception("Указан недопустимый путь (например, он ведет на несопоставленный диск)");
            }

            catch (PathTooLongException)
            {
                throw new Exception("Указанный путь, имя файла или оба значения превышают максимальную длину, заданную в системе");
            }

            catch (IOException)
            {
                throw new Exception(
                    "Параметр path включает неверный или недопустимый синтаксис имени файла, имени каталога или метки тома");
            }
            
            catch (ObjectDisposedException)
            {
                throw new Exception("Удалено средство записи потока");
            }
            catch (InvalidOperationException)
            {
                throw new Exception("Средство записи потока в настоящее время используется предыдущей операцией записи");
            }
            
        }
        
        public async Task LogCustom(string type, string message)
        {
            await WriteToFile($"{DateTime.Now:u} [{type}] {message}");
        }

        public async Task Log(LogType type, string message)
        {
            await WriteToFile($"{DateTime.Now:u} [{type.ToString()}] {message}");
        }
    }
}