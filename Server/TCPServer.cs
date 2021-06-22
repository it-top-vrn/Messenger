using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks.Dataflow;

namespace Server
{
    public class TCPServer
    {
        public IPEndPoint _ipServer;
        public int _port;
        public IPAddress _ip;
        public Socket _socket;

        //TODO уточнить наименования
        public Dictionary<string, User> clients = new Dictionary<string, User>();
        public Dictionary<string, Message> messages = new Dictionary<string, Message>();
        public Dictionary<string, List<Message>> chats = new Dictionary<string, List<Message>>();


        public TCPServer()
        {

            _ip = IPAddress.Parse("127.0.0.1");
            _port = 8005;
            _ipServer = new IPEndPoint(_ip, _port);
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public TCPServer(string ipAddress, int port)
        {
            try
            {
                _ip = IPAddress.Parse(ipAddress);
            }
            catch (ArgumentNullException)
            {
                throw new Exception("Передали пустое значение");
            }
            catch (FormatException)
            {
                throw new Exception("Передали строку неправильного формата");
            }

            if (port is < 0 or > 65535)
            {
                throw new Exception("Передали неправильный номер порта");
            }
            else
            {
                _port = port;
            }
            _ipServer = new IPEndPoint(_ip, _port);
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public bool Close()
        {
            try
            {
                _socket.Shutdown(SocketShutdown.Both);
            }
            catch (SocketException)
            {
                return false;
            }

            return true;
        }

        public string GetMessage()
        {
            var message = new StringBuilder();
            var buffer = new byte[256];

            try
            {
                do
                {
                    var bytes = _socket.Receive(buffer);
                    message.Append(Encoding.Unicode.GetString(buffer, 0, bytes));
                } while (_socket.Available > 0);
            }
            catch (Exception)
            {
                throw new Exception("Ошибка получения сообщения");
            }
            return message.ToString();
        }

        public bool SendMessage(string message)
        {
            try
            {
                var buffer = Encoding.Unicode.GetBytes(message);
                _socket.Send(buffer);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public bool SendMessageToClient(string name, Message msg)
        {
            try
            {
                var msg_send = JsonSerializer.Serialize(msg);
                clients[name].tcpclient.SendMessage(msg_send);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public Dictionary<string, User> GetClientList()
        {
            return clients;
        }

        public void AddMessage(string str, Message message)
        {
            messages.Add(str, message);
        }

        public bool SendMsgToDB(Message msg)
        {
            // Методы подключения к БДхе
            return true;
        }

        public bool SendToJournal()
        {

            // Методы журналирования    
            return true;
        }

        public bool Start()
        {
            try
            {
                _socket.Bind(_ipServer);
                _socket.Listen(10);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public User RegisterClient(TCPClient tcpclient, string nickname, string password)
        {

            var newClient = new User
            {
                tcpclient = tcpclient,
                nickname = nickname,
                password = password
            };

            try
            {
                clients.Add(nickname, newClient);
                // Добавление в DB 
                SendMessageToClient(nickname, new Message
                {
                    Date = $"{DateTime.Now:u}",
                    Msg = "Регистрация завершена успешно."

                });

                Console.WriteLine($"{nickname} {DateTime.Now:u}: Регистрация завершена успешно.");
                return newClient;
            }
            catch (ArgumentNullException)
            {
                SendMessageToClient(nickname, new Message
                {
                    Date = $"{DateTime.Now:u}",
                    Msg = "Отказ регистрации. Пожалуйста, введите никнейм и пароль."
                });
                Console.WriteLine($"Клиент {nickname} {DateTime.Now:u}: Неудачная попытка регистрации: Отсутствие данных");
                throw new Exception();
            }
            catch (ArgumentException)
            {


                SendMessageToClient(nickname, new Message
                {
                    Date = $"{DateTime.Now:u}",
                    Msg = $"Отказ регистрации. Никнейм {nickname} уже занят."
                });
                Console.WriteLine($"Клиент {nickname} {DateTime.Now:u}: Отказ регистрации. Попытка повторной регистрации.");
                throw new ArgumentException();
            }

        }

        public User Authorization(TCPClient tcpclient, string nickname, string password)
        {
            //TODO все сообщения переместить в соответствующие методы в прогрем
            if (nickname == "" || password == "")
            {
                throw new ArgumentNullException();
            }
            var newClient = new User
            {
                tcpclient = tcpclient,
                nickname = nickname,
                password = password
            };

            if (clients.ContainsKey(nickname) && clients[nickname].password == password)
            {
                SendMessageToClient(nickname, new Message
                {
                    Date = $"{DateTime.Now:u}",
                    Msg = "Авторизация завершена успешно."
                });
                Console.WriteLine($"Клиент {nickname} {DateTime.Now:u}: Авторизация завершена успешно.");
                return newClient;
            }
            else
            {
                SendMessageToClient(nickname, new Message
                {
                    Date = $"{DateTime.Now:u}",
                    Msg = "Отказ Авторизации. Неправельный логин или пароль."
                });
                Console.WriteLine($"Клиент {nickname} {DateTime.Now:u}: Неудачная попытка авторизации: Неправельный логин или пароль.");
                throw new ArgumentException();
            }
        }

        public void AuthorizeClient(TCPClient tcpclient, string nickname, string password)
        {
            var newClient = new User
            {
                tcpclient = tcpclient,
                nickname = nickname,
                password = password
            };

            try
            {
                clients.Add(nickname, newClient);
                // Добавление в DB 
                SendMessageToClient(nickname, new Message
                {
                    Date = $"{DateTime.Now:u}",
                    Msg = "Регистрация завершена успешно."

                });

                Console.WriteLine($"{nickname} {DateTime.Now:u}: Регистрация завершена успешно.");
            }
            catch (ArgumentNullException)
            {
                SendMessageToClient(nickname, new Message
                {
                    Date = $"{DateTime.Now:u}",
                    Msg = "Отказ регистрации. Пожалуйста, введите никнейм и пароль."
                });
                Console.WriteLine($"Клиент {nickname} {DateTime.Now:u}: Неудачная попытка регистрации: Отсутствие данных");
                throw new Exception();
            }
            catch (ArgumentException)
            {
                var existing_client = clients[nickname];
                if (existing_client.password == password)
                {
                    SendMessageToClient(nickname, new Message
                    {
                        Date = $"{DateTime.Now:u}",
                        Msg = "Авторизация завершена успешно."
                    });
                    Console.WriteLine($"Клиент {nickname} {DateTime.Now:u}: Авторизация завершена успешно.");
                }
                else
                {
                    SendMessageToClient(nickname, new Message
                    {
                        Date = $"{DateTime.Now:u}",
                        Msg = "Отказ Авторизации. Неправельный пароль."
                    });
                    Console.WriteLine($"Клиент {nickname} {DateTime.Now:u}: Неудачная попытка авторизации: Неправельный пароль.");
                    throw new Exception();
                }
            }

        }

        public TCPClient NewClient()
        {
            try
            {
                var client = new TCPClient(_socket.Accept());
                return client;
            }
            catch (Exception)
            {
                throw new Exception("Ошибка соединения с клиентом");
            }
        }
    }
}