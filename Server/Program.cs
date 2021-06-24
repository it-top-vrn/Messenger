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
                string nickname = "login";
                string password = "pass";
                string option = "1";
                var newClient =  new User
                {
                    nickname = nickname,
                    password = password,
                    tcpclient = newTCPClient
                };
                Console.WriteLine($"Клиент {newClient.nickname} {DateTime.Now:u}: Клиент подключился.");
                var task = Task.Run(() => TaskClient(newTCPClient));
                /* try
                 {
                     Registration_Authorization(server, newClient, option);
                 }
                 catch (Exception e)
                 {
                     server.SendMessageToClient(newClient.nickname, new Message
                     {
                         Date = $"{DateTime.Now:u}",
                         Msg = $"Выход из программы."
                     });
                     newTCPClient.Close();
                     Console.WriteLine($"Клиент {newClient.nickname} {DateTime.Now:u}: Клиент отключился.");
                     break;
                 }

                 var task = Task.Run(() => MsgHandler(server));*/
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

        /*
        static bool Registration_Authorization(TCPServer server, User newClient,string option)
        {
            var db_api = new DB_api();
            db_api.Connect();
            if (newClient.nickname == String.Empty || newClient.password == String.Empty)
            {
                server.AddClient(newClient.nickname, newClient);
                string temp = "регистрации";
                if (option == "2")
                {
                    temp = "авторизации";
                }
                // Журналирование
                server.SendMessageToClient(newClient.nickname, new Message
                {
                    Date = $"{DateTime.Now:u}",
                    Msg = "Отказ {temp}. Пожалуйста, введите никнейм и пароль."
                });
                Console.WriteLine(
                    $"Клиент {newClient.nickname} {DateTime.Now:u}: Неудачная попытка {temp}: Отсутствие данных");
                throw new Exception();
            }
            
            switch (option)
            {
                case "1":

                    try
                    {
                        Registration(server, newClient, db_api);
                    }
                    catch (Exception)
                    {

                        throw new Exception();
                    }
                    break;
                
                case "2":
                    try
                    {
                        Authorization(server, newClient, db_api);
                    }
                    catch (Exception)
                    {

                        throw new Exception();
                    }
                    break;
                
                default: 
                    break;
            }
            return true;
        }

        static void Registration(TCPServer server, User newClient, ref DB_api db_api)
        {
            if (db_api.Registration(newClient.nickname, newClient.password))
            {
                server.SendMessageToClient(newClient.nickname, new Message
                {
                    Date = $"{DateTime.Now:u}",
                    Msg = "Регистрация завершена успешно."

                });
                // Журналирование
            }
            else
            {
                server.SendMessageToClient(newClient.nickname, new Message
                {
                    Date = $"{DateTime.Now:u}",
                    Msg = $"Отказ регистрации. Никнейм {newClient.nickname} уже занят."
                });
                Console.WriteLine(
                    $"Клиент {newClient.nickname} {DateTime.Now:u}: Отказ регистрации. Попытка повторной регистрации.");
                // Журналирование
                throw new Exception();
            }
        }

        static void Authorization(TCPServer server, User newClient, ref DB_api db_api)
        {
            if (db_api.Authentication(newClient.nickname, newClient.password))
            {
                server.SendMessageToClient(newClient.nickname, new Message
                {
                    Date = $"{DateTime.Now:u}",
                    Msg = "Регистрация завершена успешно."

                });
                Console.WriteLine($"Клиент {newClient.nickname} {DateTime.Now:u}: Успешная авторизация.");

                //Журналирование успех
            }
            else
            {
                server.SendMessageToClient(newClient.nickname, new Message
                {
                    Date = $"{DateTime.Now:u}",
                    Msg = $"Отказ авторизации. Неправельный никнейм или пароль."
                });
                Console.WriteLine(
                    $"Клиент {newClient.nickname} {DateTime.Now:u}: Отказ авторизации. Неправельный никнейм или пароль.");
                // Журналирование
                throw new ArgumentException();
            }
        }
        */
        static void ClientDisconnect(User client, TCPServer server)
        {
            server.DeleteClient(client.nickname);
            client.tcpclient.Close();
            ShowInfo("Клиент отключился..."); 
            // Добаление записи в журнал
        }

        static void MsgHandler(TCPServer server)
        {
            while (true)
            {
                var msg_temp = JsonSerializer.Deserialize<Message>(server.GetMessage());

                // TODO Продумать отключение клиента. Может быть оставить такую реализацию 
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