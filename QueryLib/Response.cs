using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
	public enum ResponseType{
		RequestAccepted, RequestDenied
	}
	
    public class Response<T>
    {
        public ResponseType Type { get; set; }
		//public List<string> Contacts { get; set; }
		//public List<Message> Chat { get; set; }
		T Data { get; set; }


        public Response(){}

        public Response(T data, ResponseType type)
        {
            Data = data;
            Type = type;
        }
		
    }
}
