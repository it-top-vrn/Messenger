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

        public List<User> Contacts { get; set; }
        public List<User> Dialogs { get; set; }
    }
}
