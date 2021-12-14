using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using InfoLib;
using Logger;
using ConsoleApp10;
using System.Net.Sockets;
using System.Text;

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
                var newClient = new User(newTCPClient);
                
                
                Info.ShowLog(newClient.nickname, DateTime.UtcNow.ToString("u"), "Клиент подключился");
                var logger = new LogToFile(@"D:/log.txt");
                //logger.WriteToFile(DateTime.UtcNow.ToString("u"), $"Клиент {newClient.nickname} подключился.");
                Info.ShowInfo("Pr L32");
               
                /*
                string message = "ваше сообщение доставлено";
                byte[] data = new byte[256];
                data = Encoding.Unicode.GetBytes(message);
                newTCPClient._socket.Send(data);
                */
                var task = Task.Run(() => ServerLogic.RequestHandler(server, ref newClient));
                
            }
        }
    }
}