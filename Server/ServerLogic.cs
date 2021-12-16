using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InfoLib;
using Logger;
using ConsoleApp10;
using System.Text.Json;

namespace Server
{
    class ServerLogic
    {
        static public void RequestHandler(TCPServer server, ref User sender)
        {
            var logger = new LogToFile(@"D:\log.txt");
            var db = new DBApi();
            
            db.Connect();
            server.AddActiveClient(sender.nickname, sender);
            Request<string> request = new Request<string>();
            Request<User> userRequest = new Request<User>();
            Request<Message> messageRequest = new Request<Message>();
            bool flag = true;

            /*userRequest = JsonSerializer.Deserialize<Request<User>>(server.GetMessage());
            switch (userRequest.Type)
            {
                case RequestType.Registration:
                    if (Registration(server, userRequest.Data, ref db, logger))
                    {
                        sender.nickname = userRequest.Data.nickname;
                        sender.password = userRequest.Data.password;
                    }
                    else
                    {
                        flag = false;
                        return;
                    }

                    break;

                case RequestType.Authorization:
                    if (!Authorization(server, userRequest.Data, ref db, logger))
                    {
                        flag = false;
                        return;
                    }
                    break;
            }*/
            

            
            
            while (flag)
            {
                var temp = sender.tcpclient.GetMessage();
                
                try
                {

                    request = JsonSerializer.Deserialize<Request<string>>(temp);
                }
                catch (Exception)
                {
                    return;
                }

                
                
             
            switch (request.Type)
                {
                    case RequestType.Test:
                        Info.ShowInfo(request.Data);
                        sender.tcpclient.SendMessage("1");
                        break;

                    case RequestType.Registration:
						var user = JsonSerializer.Deserialize<User>(request.Data);
                        if (Registration(server, user, ref db, logger))
                        {
                            sender.nickname = user.nickname;
                            sender.password = user.password;
                        }
                        else
                        {
                            flag = false;
                            return;
                        }

                        break;

                    case RequestType.Authorization:
						var user = JsonSerializer.Deserialize<User>(request.Data);
                        if (!Authorization(server, user, ref db, logger))
                        {
                            flag = false;
                            return;
                        }
                        break;

                    case RequestType.Disconnect:
                        ClientDisconnect(sender.nickname, sender.tcpclient, server, logger);
                        flag = false;
                        break;

                    case RequestType.Message:
                        var msg = JsonSerializer.Deserialize<Message>(request.Data);
                        Info.ShowLog(msg.SenderNickname, msg.ReceiverNickname, msg.Msg);
                        //MessageHandler(server, messageRequest.Data, ref db, logger);
                        break;

                    case RequestType.DropTheChat:
						var receiver = JsonSerializer.Deserialize<String>(request.Data);
                        DropTheChat(sender.nickname, receiver, db, server);
                        break;

                    case RequestType.GiveMeMessageList:
                        List<Message> chat = ReturnChat(sender.nickname, request.Data, db);
                        var jsonChat = JsonSerializer.Serialize(chat);
                        var response = new Response<string>(jsonChat, ResponseType.RequestAccepted);
                        server.SendMessageToClient(sender.nickname, response);
                        break;

                    case RequestType.GiveMeContactList:
                        var conts = GiveMeContactList(sender, server, db);
                        var jsonConts = JsonSerializer.Serialize(conts);

                        var curResp = new Response<string>(jsonConts, ResponseType.RequestAccepted);
                        server.SendMessageToClient(sender.nickname, curResp);
                        break;

                    case RequestType.AddNewContact:
                        AddNewContact(sender.nickname, request.Data, db);
                        break;

                    case RequestType.DeleteTheContact:
                        DeleteContact(sender.nickname, request.Data, db);
                        break;

                    default:
                        return;
                }
            }
            return;
        }

        static bool Registration(TCPServer server, User newClient, ref DBApi db, LogToFile logger)
        {
            var date = DateTime.Now.ToString();
            if (db.Registration(newClient.nickname, newClient.password))
            {
                server.SendMessageToClient(newClient.nickname, ResponseType.RequestAccepted);
                Info.ShowLog(newClient.nickname, date, "Регистрация прошла успешно");
                //logger.WriteToFile(DateTime.UtcNow.ToString("u"), "Регистрация прошла успешно");
                return true;
            }
            else
            {
                server.SendMessageToClient(newClient.nickname, ResponseType.RequestDenied);
                Info.ShowLog(newClient.nickname, date, "Отказ регистрации. Никнейм уже занят, а может и еще какая-нибудь херня");
                //logger.WriteToFile(DateTime.UtcNow.ToString("u"), "Отказ регистрации. Никнейм уже занят, а может и еще какая-нибудь херня");
                return false;
            }
        }

        static bool Authorization(TCPServer server, User newClient, ref DBApi db, LogToFile logger)
        {
            var date = DateTime.Now.ToString();
            if (db.Authentication(newClient.nickname, newClient.password))
            {
                server.AddActiveClient(newClient.nickname, newClient);
                server.SendMessageToClient(newClient.nickname, ResponseType.RequestAccepted);
                Info.ShowLog(newClient.nickname, date, "Авторизация прошла успешно");
                //logger.WriteToFile(DateTime.UtcNow.ToString("u"), "Авторизация прошла успешно");
                return true;
            }
            else
            {
                server.SendMessageToClient(newClient.nickname, ResponseType.RequestDenied);
                Info.ShowLog(newClient.nickname, date, "Отказ авторизации. Неправельный никнейм или пароль");
              //  logger.WriteToFile(DateTime.UtcNow.ToString("u"), "Отказ авторизации. Неправельный никнейм или пароль");
                return false;
            }
        }

        static void ClientDisconnect(string sender, TCPClient client, TCPServer server, LogToFile logger)
        {
            server.SendMessageToClient(sender, ResponseType.RequestAccepted);
            client.Close();
            server.ActiveClients.Remove(sender);
            Info.ShowLog(sender, DateTime.UtcNow.ToString("u"), "Клиент отключился.");
            //logger.WriteToFile(DateTime.UtcNow.ToString("u"), $"Клиент {sender} отключился.");
        }

        static void DropTheChat(string sender, string receiver, DBApi db, TCPServer server)
        {
            db.Drop(sender, receiver);
            server.SendMessageToClient(sender, ResponseType.RequestAccepted);
        }

        static List<Message> ReturnChat(string sender, string receiver, DBApi db)
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
                    Msg = msg.Value
                };

                chat.Add(msgObject);
            }
            return chat;
        }

        static void MessageHandler(TCPServer server, Message msg_temp, DBApi db, LogToFile logger)
        {
            var sender = msg_temp.SenderNickname;
            var receiver = msg_temp.ReceiverNickname;
            var msg = new Message
            {
                SenderNickname = sender,
                ReceiverNickname = receiver,
                Date = $"{DateTime.Now:u}",
                Msg = $"Вы {msg_temp.Date}: {msg_temp.Msg}"
            };
            Info.ShowInfo($"{sender} to  {receiver} : {msg_temp.Msg}");
			if(!(db.InsertMessage(sender, receiver, DateTime.UtcNow.ToString("u"), msg.Msg)){
				Info.ShowLog(sender, DateTime.UtcNow.ToString("u"), $"Запись в БД сообщения не удалась.");
				//logger.WriteToFile(DateTime.UtcNow.ToString("u"), $"Запись в БД сообщения не удалась.");
			}
			
            var response = new Response<Message>(msg, ResponseType.RequestAccepted);
            if (!server.SendMessageToClient(sender, response))
            {
                Info.ShowLog(sender, DateTime.UtcNow.ToString("u"), $"Передача ответа потерпела фиаско.");
                //logger.WriteToFile(DateTime.UtcNow.ToString("u"), $"Передача ответа клиенту {sender} потерпела фиаско.");
                
            }

            msg.Msg = $"{sender} {msg_temp.Date}: {msg_temp.Msg}";
            response = new Response<Message>(msg, ResponseType.RequestAccepted);
            if (!server.SendMessageToClient(receiver, response))
            {
                Info.ShowLog(receiver, DateTime.UtcNow.ToString("u"), $"Передача ответа потерпела фиаско.");
                //logger.WriteToFile(DateTime.UtcNow.ToString("u"), $"Передача ответа клиенту {receiver} потерпела фиаско.");
            }
            
        }

        static public List<string> GiveMeContactList(User client, TCPServer server, DBApi db)
        {
            return db.GetContactsList(client.nickname);
        }

        static public void AddNewContact(string sender, string newContact, DBApi db)
        {
            db.InsertContact(sender, newContact);
        }

        static public void DeleteContact(string sender, string contact, DBApi db)
        {
            //db.DeleteContact(sender, contact);
        }

        static List<Message> GiveMeMassegeList(string sender, string receiver, DBApi db)
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
                        Msg = msg.Value
                    }
                );
            }
            return list;
        }
    }
}