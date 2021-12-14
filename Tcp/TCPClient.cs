using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace Server
{
    public class TCPClient
    {
        public IPEndPoint _ipServer;
        public int _port;
        public IPAddress _ip;
        public Socket _socket;

        public TCPClient()
        {
            _ip = IPAddress.Parse("127.0.0.1");
            _port = 8005;
            _ipServer = new IPEndPoint(_ip, _port);
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public TCPClient(Socket socket)
        {
            _socket = socket ?? throw new Exception("Ошибка создания клиента");
        }

        public TCPClient(string ipAddress, int port)
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

        public bool Connect()
        {
            try
            {
                _socket.Connect(_ipServer);
            }
            catch (SocketException)
            {
                return false;
            }

            return true;
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
            catch (ArgumentNullException)
            {

            }
            catch (SocketException)
            {

            }
            catch (ArgumentOutOfRangeException)
            {

            }
            catch (ArgumentException)
            {

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

        public bool SendMessageToClient(string name, ResponseType response)
        {
            try
            {
                var msg_send = JsonSerializer.Serialize(response);
                //ActiveClients[name].tcpclient.SendMessage(msg_send);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
}