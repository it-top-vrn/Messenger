using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace DB_API
{
    public class DBApi2
    {
        private readonly DBContext _db;

        public DBApi2()
        {
            _db = DBContext.Init();
        }

        public void AddNewClient(ClientTable client)
        {
            _db.Client_table.Add(client);
            _db.SaveChanges();
        }

        public void ClientUpdate(ClientTable client)
        {
            _db.Client_table.Update(client);
            _db.SaveChanges();
        }

        public void ClientDelete()
        {
            //TODO придумать, как удалять
        }

        public List<string> GiveMeContactList(ClientTable client)
        {
            var allContacts = _db.Contact_table;
            var contactList = new List<string>();

            contactList = (from contact in allContacts
                          where contact.ClientID == client.ID
                          select contact.ContactName).ToList<string>();

            return contactList;
        }

        public void AddNewContact(ContactTable newContact)
        {
            _db.Contact_table.Add(newContact);
            _db.SaveChanges();
        }

        public void ClientUpdate(ContactTable newContact)
        {
            _db.Contact_table.Update(newContact);
            _db.SaveChanges();
        }

        public void ContactDelete()
        {
            //TODO придумать, как удалять
        }


        public void Dispose()
        {
            _db?.Dispose();
        }
    }
}
