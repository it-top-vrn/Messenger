using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
	enum RequestType{
		Registration, Authorization, Disconnect, Message, AddNewContact, DeleteTheContact, DropTheChat, GiveMeContactList, GiveMeMessageList
	}
	
    public class Request<T>
    {
        RequestType Type { get; set; }
		User Client { get; set; }
		Message Msg { get; set; }
		

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
