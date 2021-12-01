using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using InfoLib;
;

namespace Server
{
    internal static class Program
    {
        private static void Main()
        {
            Info.ShowInfo("Сервер запущен");
            var server = new TCPServer("127.0.0.1", 8005);
            server.Start();
            var someShitAssFlag = true;
            Info. ShowInfo("Ожидаю подключение...");
            while (someShitAssFlag)
            {
                var newTCPClient = server.NewClient();
                var newClient =  new User
                {
                    tcpclient = newTCPClient
                };
                newClient.tcpclient.SendMessage("1");
                Info.ShowLog(newClient.nickname, DateTime.UtcNow.ToString("u"), "Клиент подключился");
                var logger = new LogToFile();
                logger.WriteToFile(DateTime.UtcNow.ToString("u"), $"Клиент {newClient.nickname} подключился.");
                
                var task = Task.Run(() => ServerLogic.RequestHandler(server, ref newClient));
            }
        }
    }
}