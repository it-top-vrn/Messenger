using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    public class TCPServer
    {
        protected IPEndPoint _ipServer;
        protected int _port;
        protected IPAddress _ip;
        protected Socket _socket;

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

        /*public TCPClient NewClient()
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
        }*/
    }
}