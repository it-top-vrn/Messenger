using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Request<T>
    {
        string Type { get; set; }
        string Msg { get; set; }

        T Data { get; set; }

        Request()
        {

        }

        public Request(T data, string type)
        {
            Type = type;
            Data = data;
        }
    }
}
