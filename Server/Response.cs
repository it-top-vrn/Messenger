using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
	enum ResponseType{
		RequestAccepted, RequestDenied
	}
	
    public class Response
    {
        ResponseType Type { get; set; }
		List<string> Contacts { get; set; }
		List<Message> Chat { get; set; }

        public Response(){}

        
		
    }
}
