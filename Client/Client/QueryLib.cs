using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public enum RequestType
    {
        Registration, Authorization, Disconnect, Message, AddNewContact, DeleteTheContact, DropTheChat,
        GiveMeContactList, GiveMeChatList, GiveMeMessageList, ChangePassword, ChangeNickName
    }
    
    public enum ResponseType
    {
        RequestAccepted, RequestDenied
    }

    public class QueryLib<T>
    {
        public RequestType rqType { get; set; }
        public ResponseType rsType { get; set; }
        //public User Client { get; set; }
        //public Message Msg { get; set; }

        public T Data { get; set; }

        public QueryLib(QueryLib<string> queryLib)
        {

        }

        public QueryLib(T data, RequestType type)
        {
            rqType = type;
            Data = data;
        }
        
        public QueryLib(T data, ResponseType type)
        {
            rsType = type;
            Data = data;
        }
        public QueryLib(T data)
        {
            Data = data;
        }
    }
}