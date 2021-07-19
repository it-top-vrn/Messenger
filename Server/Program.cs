﻿using System;
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
		
		//TODO перенести в отдельный класс ShowMeSomeShitAssInfo
		static void ShowInfo (string message)
        {
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine(message);
            Console.ResetColor();
        }
		
		static void ShowLog (string sender, string date, string message){
			Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Клиент {sender} at {date}: {message}.");
            Console.ResetColor();
		}
		
		static void ShowMessage (string sender, string receiver, string date, string message){
			Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{sender} to {receiver} at {date}: {message}.");
            Console.ResetColor();
		}


		//TODO Тоже перенести в отдельный класс, HandleMeSomeShitAssRequests 
        static bool RequestHandler(TCPServer server, ref User sender)
        {
            var db = new DB_api();
            db.Connect();
            
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
                        ClientDisconnect(sender.nickname, server);
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
				ShowLog(newClient.nickname, "Регистрация прошла успешно");
                return true;
            }
            else
            {
                server.SendMessageToClient(newClient.nickname, ResponseType.RequestDenied);
				ShowLog(newClient.nickname, "Отказ регистрации. Никнейм уже занят, а может и еще какая-нибудь херня");
                // Журналирование
                return false;
            }
        }

        static bool Authorization(TCPServer server, User newClient, ref DB_api db, LogToFile logger)
        {
            if (db.Authentication(newClient.nickname, newClient.password))
            {
                server.SendMessageToClient(newClient.nickname, ResponseType.RequestAccepted);
                // Журналирование
				ShowLog(newClient.nickname, "Авторизация прошла успешно");
                return true;
            }
            else
            {
                server.SendMessageToClient(newClient.nickname, ResponseType.RequestDenied);
				ShowLog(newClient.nickname, "Отказ авторизации. Неправельный никнейм или пароль");
                // Журналирование
                throw new ArgumentException();
                return false;
            }
        }
        
        static void ClientDisconnect(string sender, TCPServer server)
        {
            server.ActiveClients.Remove(sender);			
			var response = new Response<string>{Type = ResponseType.RequestAccepted};
            server.SendMessageToClient(sender, response);
            client.tcpclient.Close();
            Console.WriteLine($"Клиент {sender} {DateTime.Now:u}: Клиент отключился.");
            // Добаление записи в журнал
        }

        //TODO исправить
        static void DropTheChat(string sender, string receiver, DB_API db, TCPServer server)
        {

            if(db.Drop(sender, receiver)){
				server.SendMessageToClient(newClient.nickname, ResponseType.RequestAccepted);
				Console.WriteLine($"Клиент {sender} {DateTime.Now:u}: Удаление беседы {sender}-{receiver}.");
				// журналирование
			} else {
				server.SendMessageToClient(newClient.nickname, ResponseType.RequestDenied);
				Console.WriteLine($"Клиент {sender} {DateTime.Now:u}: Ошибка при удалениие беседы {sender}-{receiver}.");
				// журналирование
			}
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

        static bool MessageHandler(TCPServer server, Message msg_temp)
        {
            var sender = msg_temp.SenderNickname;
            var receiver = msg_temp.ReceiverNickname;
			var msg = new Message{
				SenderNickname = sender,
				ReceiverNickname = receiver,
				Date = $"{DateTime.Now:u}",
				Msg = $"Вы {msg_temp.Date}: {msg_temp.Msg}"
			}
            ShowInfo($"{sender} to  {receiver} : {msg_temp.Msg}");
			
			var response = new Response(sender, msg);
            if(!server.SendMessageToClient(sender, response)){
				return false;
			}
			
			msg.Msg =  $"{sender} {msg_temp.Date}: {msg_temp.Msg}";
			var response = new Response(sender, msg);
            if(!server.SendMessageToClient(receiver, response)){
				return false;
			}
			
			return true;
        }

        static public List<string> GiveMeContactList(User client, TCPServer server, DB_api db)
        {
           //Журналирование 
           return db.GetContactList(client.nickname);
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