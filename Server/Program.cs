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
            var server = new TCPServer("127.0.0.1", 8005);
            server.Start();
            var someShitAssFlag = true;
            ShowInfo("Ожидаю подключение...");
            while (someShitAssFlag)
            {
                var newTCPClient = server.NewClient();
                var newClient =  new User
                {
                    tcpclient = newTCPClient
                };
                newClient.tcpclient.SendMessage("1");
                Console.WriteLine($"Клиент {newClient.nickname} {DateTime.Now:u}: Клиент подключился.");
                
                 var task = Task.Run(() => RequestHandler(server, ref newClient));
            }
        }


        static bool RequestHandler(TCPServer server, ref User newClient)
        {
            var db = new DB_api();
            db.Connect();
            User sender = new User();
            User receiver = new User();
            Request<string> request = new Request<string>();
            Request<User> userRequest = new Request<User>();
			Request<Message> messageRequest = new Request<Message>();
            var logger = new LogToFile();
            bool flag = true;

            while (flag)
            {
                try
				{
					request = JsonSerializer.Deserialize<Request<string>>(server.GetMessage());
				}
				catch(JsonException)
				{
					try
					{
						messageRequest = JsonSerializer.Deserialize<Request<Message>>(server.GetMessage());
					}
					catch(JsonException)
					{
						try
						{
							userRequest = JsonSerializer.Deserialize<Request<User>>(server.GetMessage());
						}
						catch(JsonException)
						{
							return false;
						}
					}
				}

                switch (request.Type)
                {
                    case RequestType.Registration:
                        if(Registration(server, userRequest.Data, db, logger))
                        {
                            sender.nickname = userRequest.Data.nickname;
                            sender.password = userRequest.Data.password;
                        } else
                        {
                            flag = false;
                            return false;
                        }
                        
                        break;

                    case RequestType.Authorization:
                        if(!Authorization(server, userRequest.Data, db, logger))
                        {
                            flag = false;
                            return false;
                        }
                        break;

                    case RequestType.Disconnect:
                        ClientDisconnect(messageRequest.Data.SenderNickname, newClient, server);
                        flag = false;
                        break;

                    case RequestType.Message:
                        MessageHandler(server);
                        break;

                    case RequestType.DropTheChat:
                        DropTheChat(sender.nickname, request.Data, db, server);
                        break;

                    case RequestType.GiveMeMessageList: 
                        List<Message> chat = ReturnChat(sender.nickname, request.Data, db);
                        var response = new Response<List<Message>> ( chat, ResponseType.RequestAccepted );
                        server.SendMessageToClient(sender.nickname, response);
                        break;
                        

                    case RequestType.GiveMeContactList:
                        //db_api.GetClients()
                        List<User> contacts = new List<User>();
                        var response_2 = new Response<List<User>>(contacts, ResponseType.RequestAccepted);
                        server.SendMessageToClient(sender.nickname, response_2);
                        break;

                    case RequestType.AddNewContact:
                        //Add new Contact
						
                        break;

                    case RequestType.DeleteTheContact:
                        //delete contact
                        break;

                    default:
                        return true;
                }  
            }	
            return true;
        }


        static bool Registration(TCPServer server, User newClient, ref DB_api db, LogToFile logger)
        {
            if (db.Registration(newClient.nickname, newClient.password))
            {
				
                server.SendMessageToClient(newClient.nickname, ResponseType.RequestAccepted);
                // Журналирование
				
                return true;
            }
            else
            {
                server.SendMessageToClient(newClient.nickname, ResponseType.RequestDenied);
                // Журналирование
                return false;
            }
        }

        static bool Authorization(TCPServer server, User newClient, ref DB_api db, LogToFile logger)
        {
            if (db.Authentication(newClient.nickname, newClient.password))
            {
                server.SendMessageToClient(newClient.nickname, new Message
                {
                    Date = $"{DateTime.Now:u}",
                    Msg = "Регистрация завершена успешно."

                });
                Console.WriteLine($"Клиент {newClient.nickname} {DateTime.Now:u}: Успешная авторизация.");

                //Журналирование успех
                return true;
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
                return false;
            }
        }
        
        //TODO исправить
        static void ClientDisconnect(string sender, User client, TCPServer server)
        {
            server.ActiveClients.Remove(client.nickname);
            var request = new Request<string> { 
                Type = "5",
                Data = "Выход из аккаунта"
            };
            server.SendMessageToClient(sender, request);
            client.tcpclient.Close();
            Console.WriteLine($"Клиент {sender} {DateTime.Now:u}: Клиент отключился.");
            // Добаление записи в журнал
        }

        //TODO исправить
        static void DropTheChat(string sender, string receiver, DB_API db, TCPServer server)
        {

            db.Drop(sender, receiver);
            var msg = new Message
            {
                Date = $"{DateTime.Now:u}",
                Msg = $"Беседа с {receiver} удалена."
            };
            server.SendMessageToClient(sender, msg);
            Console.WriteLine($"Клиент {sender} {DateTime.Now:u}: Удаление беседы {sender}-{receiver}.");
            // журналирование
        }

        static List<Message> ReturnChat (string sender, string receiver, DB_API db)
        {
            List<Message> chat = new List<Message>();
            var msgList = db.GetMsgList(sender, receiver);
            foreach (var msg in msgList)
            {
                var msgObject = new Message
                {
                    SenderNickname = sender,
                    ReceiverNickname = receiver,
                    Date = $"{DateTime.Now:u}",
                    Msg = msg
                };

                chat.Add(msgObject);
            }
            return chat;
        }

        static List<User> ReturnContacs(string sender,  DB_API db)
        {
			//Убрать лишнее
            List<User> clientList = db.GetClientList(); // нужен метод возвращающий лист клиентов из БД
            List<string> contactList = db.GetContactList(sender);
            List<User> contacts = new List<User>();

            foreach (var nickname in contactList)
            {
                foreach (var client in clientList)
                {
                    if(client.nickname == nickname)
                    {
                        contacts.Add(client);
                    }
                }
            }

            return contacts;
        }

        static void MessageHandler(TCPServer server)
        {
            while (true)
            {
                var msg_temp = JsonSerializer.Deserialize<Message>(server.GetMessage());
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

        static public List<string> GiveMeContactList(User client, TCPServer server, DB_api db)
        {
           //Журналирование 
           return db.GetContactList(client.nickname);
        }

        static void ShowInfo (string message)
        {
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        static List<string> GiveMeMassegeList(string sender, string receiver, DB_api db)
        {
            var msgList = db.GetMsgList(sender, receiver);
            var list = new List<Message>();
            foreach (var msg in msgList)
            {
                list.Add(
                    new Message
                    {
                        Date = $"{DateTime.Now:u}",
                        ReceiverNickname = receiver,
                        SenderNickname = sender,
                        Msg = msg
                    }
                );
            }

            return db.GetMsgList(sender, receiver);
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