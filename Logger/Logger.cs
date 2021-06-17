using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace Logger
{ 
    public delegate void Message(string message);
    public class LogToFile
    {
        public event Message msg;
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
                msg?.Invoke("Отказано в доступе");
                throw new Exception("Отказано в доступе");
            }

            catch (ArgumentException)
            {
                msg?.Invoke("Параметр path пуст");
                throw new Exception("Параметр path пуст");
            }

            catch (DirectoryNotFoundException)
            {
                msg?.Invoke("Указан недопустимый путь (например, он ведет на несопоставленный диск)");
                throw new Exception("Указан недопустимый путь (например, он ведет на несопоставленный диск)");
            }

            catch (PathTooLongException)
            {
                msg?.Invoke("Указанный путь, имя файла или оба значения превышают максимальную длину, заданную в системе");
                throw new Exception("Указанный путь, имя файла или оба значения превышают максимальную длину, заданную в системе");
            }

            catch (IOException)
            {
                msg?.Invoke("Параметр path включает неверный или недопустимый синтаксис имени файла, имени каталога или метки тома");
                throw new Exception(
                    "Параметр path включает неверный или недопустимый синтаксис имени файла, имени каталога или метки тома");
            }
            
            catch (ObjectDisposedException)
            {
                msg?.Invoke("Удалено средство записи потока");
                throw new Exception("Удалено средство записи потока");
            }
            catch (InvalidOperationException)
            {
                msg?.Invoke("Средство записи потока в настоящее время используется предыдущей операцией записи");
                throw new Exception("Средство записи потока в настоящее время используется предыдущей операцией записи");
            }
            
        }
        
        public async Task LogInfo(string message)
        {
            await WriteToFile($"{DateTime.Now:u} [INFO] {message}");
        }
        
        public async Task LogError(string message)
        {
            await WriteToFile($"{DateTime.Now:u} [ERROR] {message}");
        }
        
        public async Task LogWarning(string message)
        {
            await WriteToFile($"{DateTime.Now:u} [WARNING] {message}");
        }
        
        public async Task LogSuccess(string message)
        {
            await WriteToFile($"{DateTime.Now:u} [SUCCESS] {message}");
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
public class LogToDB
{
    public event Message mes;
            
            public void WriteToDB(string kind, string message)
            {
                try
                {
                    using (var connection = new SqliteConnection("Data Source=logdata.db"))

                    {
                        connection.Open();

                        SqliteCommand command = new SqliteCommand();
                        command.Connection = connection;
                        command.CommandText =
                            $"INSERT INTO table_log (kinds, messages) VALUES ('{kind}', '{message}');";
                        command.ExecuteNonQuery();
                    }
                }
                catch (InvalidOperationException)
                {
                    mes?.Invoke("Невозможно открыть подключение без указания источника данных или сервера или  подключение уже открыто");
                    throw new Exception(
                        "Невозможно открыть подключение без указания источника данных или сервера или  подключение уже открыто.");
                }
                catch (SqliteException)
                {
                    
                    mes?.Invoke("Ошибка sqlite во время выполнения");
                    throw new Exception("Ошибка sqlite во время выполнения");
                }
            }
        }

}