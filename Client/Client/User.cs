using System;
using System.Collections.Generic;
using System.Text;

namespace Client
{
    public class User
    {
       public string nickName { get; set; }
       public string password { get; set; }
       public string role { get; set; }
       public string idDialog { get; set; } 

        //айди выбранного пользователем диалога, если пользователь захочет перейти во вкладку со старым чатом 
        //по умолчанию unfinded

        //public bool isOnline{ get; set; }

        public List<User> Contacts { get; set; }
        public List<Dialog> Dialogs { get; set; }

        public User() { }

        public User(string _nickName)
        {
            nickName = _nickName;
        }
    }
}
