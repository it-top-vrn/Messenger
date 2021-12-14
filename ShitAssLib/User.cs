using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class User
    {
        public TCPClient tcpclient = new TCPClient();
        public string nickname = "some nickname";
        public string password = "some password";


        public void AddTCPClient(TCPClient client)
        {
            tcpclient = (TCPClient)client;
        }
    }
}
