using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
	public enum RequestType{
		Registration, Authorization, Disconnect, Message, AddNewContact, DeleteTheContact, DropTheChat, GiveMeContactList, GiveMeMessageList
	}
	
    public class Request<T>
    {
        public RequestType Type { get; set; }
		//public User Client { get; set; }
		//public Message Msg { get; set; }
		

        public T Data { get; set; }

        public Request()
        {
			
        }

        public Request(T data, RequestType type)
        {
            Type = type;
            Data = data;
        }
    }
}
