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
            Info.ShowInfo("Srv L18");
            var db = new DBApi();
            Info.ShowInfo("Srv L18");
            db.Connect();
            
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
                try
                {
                    request = JsonSerializer.Deserialize<Request<string>>(server.GetMessage(sender));
                }
                catch (JsonException)
                {
                    try
                    {
                        messageRequest = JsonSerializer.Deserialize<Request<Message>>(server.GetMessage(sender));
                    }
                    catch (JsonException)
                    {
                        return;
                    }
                }
                
             
            switch (request.Type)
                {
                    case RequestType.Test:
                        Info.ShowInfo(request.Data);
                        break;

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

                    case RequestType.Disconnect:
                        ClientDisconnect(sender.nickname, sender.tcpclient, server, logger);
                        flag = false;
                        break;

                    case RequestType.Message:
                        MessageHandler(server, messageRequest.Data, logger);
                        break;

                    case RequestType.DropTheChat:
                        DropTheChat(sender.nickname, request.Data, db, server);
                        break;

                    case RequestType.GiveMeMessageList:
                        List<Message> chat = ReturnChat(sender.nickname, request.Data, db);
                        var response = new Response<List<Message>>(chat, ResponseType.RequestAccepted);
                        server.SendMessageToClient(sender.nickname, response);
                        break;

                    case RequestType.GiveMeContactList:
                        var conts = GiveMeContactList(sender, server, db);
                        var curResp = new Response<List<string>>(conts, ResponseType.RequestAccepted);
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
                logger.WriteToFile(DateTime.UtcNow.ToString("u"), "Регистрация прошла успешно");
                return true;
            }
            else
            {
                server.SendMessageToClient(newClient.nickname, ResponseType.RequestDenied);
                Info.ShowLog(newClient.nickname, date, "Отказ регистрации. Никнейм уже занят, а может и еще какая-нибудь херня");
                logger.WriteToFile(DateTime.UtcNow.ToString("u"), "Отказ регистрации. Никнейм уже занят, а может и еще какая-нибудь херня");
                return false;
            }
        }

        static bool Authorization(TCPServer server, User newClient, ref DBApi db, LogToFile logger)
        {
            var date = DateTime.Now.ToString();
            if (db.Authentication(newClient.nickname, newClient.password))
            {
                server.SendMessageToClient(newClient.nickname, ResponseType.RequestAccepted);
                Info.ShowLog(newClient.nickname, date, "Авторизация прошла успешно");
                logger.WriteToFile(DateTime.UtcNow.ToString("u"), "Авторизация прошла успешно");
                return true;
            }
            else
            {
                server.SendMessageToClient(newClient.nickname, ResponseType.RequestDenied);
                Info.ShowLog(newClient.nickname, date, "Отказ авторизации. Неправельный никнейм или пароль");
                logger.WriteToFile(DateTime.UtcNow.ToString("u"), "Отказ авторизации. Неправельный никнейм или пароль");
                return false;
            }
        }

        static void ClientDisconnect(string sender, TCPClient client, TCPServer server, LogToFile logger)
        {
            server.ActiveClients.Remove(sender);
            server.SendMessageToClient(sender, ResponseType.RequestAccepted);
            client.Close();
            Info.ShowLog(sender, DateTime.UtcNow.ToString("u"), "Клиент отключился.");
            logger.WriteToFile(DateTime.UtcNow.ToString("u"), $"Клиент {sender} отключился.");
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

        static void MessageHandler(TCPServer server, Message msg_temp, LogToFile logger)
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

            var response = new Response<Message>(msg, ResponseType.RequestAccepted);
            if (!server.SendMessageToClient(sender, response))
            {
                Info.ShowLog(sender, DateTime.UtcNow.ToString("u"), $"Передача ответа потерпела фиаско.");
                logger.WriteToFile(DateTime.UtcNow.ToString("u"), $"Передача ответа клиенту {sender} потерпела фиаско.");
                
            }

            msg.Msg = $"{sender} {msg_temp.Date}: {msg_temp.Msg}";
            response = new Response<Message>(msg, ResponseType.RequestAccepted);
            if (!server.SendMessageToClient(receiver, response))
            {
                Info.ShowLog(receiver, DateTime.UtcNow.ToString("u"), $"Передача ответа потерпела фиаско.");
                logger.WriteToFile(DateTime.UtcNow.ToString("u"), $"Передача ответа клиенту {receiver} потерпела фиаско.");
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