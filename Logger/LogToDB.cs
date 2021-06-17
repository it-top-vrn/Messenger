using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace Logger
{
    public class LogToDB
    {
        public event Message Mes;

        public void WriteToDb(string kind, string message)
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
                Mes?.Invoke(
                    "Невозможно открыть подключение без указания источника данных или сервера или  подключение уже открыто");
                throw new Exception(
                    "Невозможно открыть подключение без указания источника данных или сервера или  подключение уже открыто.");
            }
            catch (SqliteException)
            {
                Mes?.Invoke("Ошибка sqlite во время выполнения");
                throw new Exception("Ошибка sqlite во время выполнения");
            }
        }
    }
}