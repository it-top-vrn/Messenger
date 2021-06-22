using System;
using System.Collections.Generic;
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
                
                var newTCPClient = server.NewClient();
                 
                try
                {
                    var temp = server.GetMessage();
                    string nickname = "login";
                    string password = "pass";
                    server.AuthorizeClient(newTCPClient, nickname, password);
                }
                catch (Exception e)
                {
                    break;
                }

                var client = server.NewClient();

                var task = Task.Run(() => TaskClient(client));
            }
        }


        /*static User TaskClient(TCPClient client)
        {
            var newClient = JsonSerializer.Deserialize<User>(client);
            ShowInfo($"Клиент {msg_temp.name} подключился");
            //Добавление нового клиента в таблицу клиентов
            //Добавление записи о подключении в журнал
        }*/


        static void Registration()
        {

        }






        static void Authorization(string client, ref TCPServer server)
        {
            var newClient = JsonSerializer.Deserialize<User>(client);
            try
            {
               // server.AuthorizeClient(newClient);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        static void ClientConnect(string client, ref TCPServer server)
        {
            var newClient = JsonSerializer.Deserialize<User>(client);
            //server.AddClient(newClient);
            ShowInfo($"Клиент {newClient.nickname} подключился");
            //Добавление нового клиента в таблицу клиентов
            //Добавление записи о подключении в журнал
        }

        static void ClientDisconnect(TCPClient client)
        {
            client.Close();
            ShowInfo("Клиент отключился...");
            // Изменение статуса в БД
            // Добаление записи в журнал
        }

        static void MsgRecever(TCPServer server)
        {
            while (true)
            {
                var msg_temp = JsonSerializer.Deserialize<Message>(server.GetMessage());

                // TODO Продумать открючение клиента. Может быть оставить такую реализацию 
                /*if (msg_temp.Type == TypeMessage.Stop)
                {
                    ShowInfo("Клиент отключился...");
                    break;
                }*/

                var sender_name = msg_temp.SenderNickname;
                var receiver_name = msg_temp.ReceiverNickname;

                ShowInfo($"{sender_name} to  {receiver_name} : {msg_temp.Msg}");
                server.SendMessageToClient(sender_name, new Message
                {
                    Date = $"{DateTime.Now:u}",
                    Msg = $"Вы {msg_temp.Date}: {msg_temp.Msg}"
                });

                server.SendMessageToClient(receiver_name, new Message
                {
                    Date = $"{DateTime.Now:u}",
                    Msg = $"{sender_name} {msg_temp.Date}: {msg_temp.Msg}"
                });
            }
        }

        static void TaskClient(TCPClient client)
        {
            //var newClient = JsonSerializer.Deserialize<User>(client);
            while (true)
            {

            }
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
                //Данные класса
            };
            var msg_send = JsonSerializer.Serialize(msg);
            return msg_send;
        }


    }
}