using System;
using System.Text.Json;
using System.Threading.Tasks;


namespace Server
{
    internal static class Program
    {
        private static void Main()
        {
            ShowInfo("Сервер запущен");
            var server = new TCPServer("192.168.0.168", 8005);
            server.Start();
            ShowInfo("Ожидаю подключение...");
            while (true)
            {
                //var client = server.NewClient();

                //var task = Task.Run(() => TaskClient(client));
            }
        }

        static void TaskClient(TCPClient client)
        {
            var name = "Анон";
            var name_temp = JsonSerializer.Deserialize<Message>(client.GetMessage());
            if (name_temp.Type == TypeMessage.Name)
            {
                name = name_temp.Msg;
                ShowInfo($"Клиент {name} подключился");
            }

            while (true)
            {
                var msg_temp = JsonSerializer.Deserialize<Message>(client.GetMessage());
                if (msg_temp.Type == TypeMessage.Stop)
                {
                    ShowInfo("Клиент отключился...");
                    break;
                }

                if (msg_temp.Type == TypeMessage.Message)
                {
                    ShowInfo($"Сообщение от {name}: {msg_temp.Msg}");
                }

                client.SendMessage(MessageTypeMessage("Сообщение получено"));
            }
            client.Close();
        }

        static void ShowInfo(string message)
        {
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        static string MessageTypeMessage(string message)
        {
            var msg = new Message
            {
                Type = TypeMessage.Message,
                Msg = message
            };
            var msg_send = JsonSerializer.Serialize(msg);
            return msg_send;
        }
    }
}