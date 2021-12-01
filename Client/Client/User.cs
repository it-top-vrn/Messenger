using System;
using System.Collections.Generic;
using System.Text;

namespace Client
{
    public class User
    {
       public string nickName;
       public string password;
       public string idUser;
       public string role;

       public List<Dialog> dialogs;
       public List<User> contacts;
    }
}
