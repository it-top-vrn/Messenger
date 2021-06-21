using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using MySql.Data.MySqlClient;

namespace ConsoleApp10
{
    public class DB_api
    {
        private string temp;
        private string sender;
        private string recepietnt;


        const string connection =
            " Server=mysql60.hostland.ru;    Database=host1323541_vrn02;   Uid=host1323541_itstep;   Pwd=269f43dc;   ";

        MySqlConnection db = new MySqlConnection(connection);

        public void Connect()
        {
            db = new MySqlConnection(connection);
            db.Open();
        }


        public bool InsertMessage(string senderID, string recepientID, string time, string msg)
        {
            sender = senderID;
            recepietnt = recepientID;

            db.Open();
            MySqlCommand check_table = new MySqlCommand($"SELECT * from {sender + "_" + recepietnt}", db);

            try
            {
                check_table.ExecuteNonQuery();
                Console.WriteLine("table " + sender + "_" + recepietnt + " found");
            }
            catch (Exception)
            {
                Console.WriteLine("table " + sender + "_" + recepietnt + " NOT found");
                temp = sender;
                sender = recepietnt;
                recepietnt = temp;
                MySqlCommand check_table2 = new MySqlCommand($"SELECT * from {sender + "_" + recepietnt}", db);
                try
                {
                    check_table2.ExecuteNonQuery();
                    Console.WriteLine("table " + sender + "_" + recepietnt + " found");
                }
                catch (Exception)
                {
                    Console.WriteLine("table " + sender + "_" + recepietnt + " NOT found");
                    temp = sender;
                    sender = recepietnt;
                    recepietnt = temp;
                }
            }

            bool check = false;
            MySqlCommand command =
                new MySqlCommand(
                    $"INSERT INTO {sender + "_" + recepietnt} (senderID, recepientID, Message_Time , MESSAGE) VALUES ('{senderID}', '{recepientID}', '{time}', '{msg}')",
                    db);
            try
            {
                command.ExecuteNonQuery();
                check = true;
            }
            catch (Exception e)
            {
                MySqlCommand create_table =
                    new MySqlCommand(
                        $"CREATE TABLE  {sender + "_" + recepietnt} (Id INT PRIMARY KEY AUTO_INCREMENT NOT NULL, recepientID VARCHAR(20) , senderID VARCHAR(20), Message_Time VARCHAR(20), MESSAGE VARCHAR(500))",
                        db);
                create_table.ExecuteNonQuery();
                command.ExecuteNonQuery();
                check = true;
            }

            db.Close();
            return check;
        }


        public bool Authentication(string nickName, string password)
        {
            bool auth = false;
            db.Open();
            DataTable table = new DataTable();
            MySqlDataAdapter adapter = new MySqlDataAdapter();
            MySqlCommand command =
                new MySqlCommand($"SELECT * FROM table_USER WHERE NICKNAME = '{nickName}' AND PASSWORD = '{password}'",
                    db);
            adapter.SelectCommand = command;
            adapter.Fill(table);
            if (table.Rows.Count > 0)
            {
                auth = true;
            }

            db.Close();
            return auth;
        }

        public bool Registration(string nickName, string password)
        {
            bool check = false;
            db.Open();


            MySqlCommand exists = new MySqlCommand($"SELECT * FROM table_USER WHERE NICKNAME = '{nickName}'", db);
            DataTable table = new DataTable();
            MySqlDataAdapter adapter = new MySqlDataAdapter();
            adapter.SelectCommand = exists;
            adapter.Fill(table);
            if (table.Rows.Count > 0)
            {
                // Console.WriteLine("THIS NICKNAME IS ALREADY EXISTS");
            }

            if (table.Rows.Count == 0)
            {
                MySqlCommand command =
                    new MySqlCommand(
                        $"INSERT INTO table_USER ( NICKNAME, PASSWORD) VALUES ( '{nickName}', '{password}')", db);
                if (command.ExecuteNonQuery() == 1)
                {
                    check = true;
                }
            }

            db.Close();
            return check;
        }


        public List<KeyValuePair<string, string>> GetMsgList(string senderID, string recepientID)
        {
            List<KeyValuePair<string, string>> msgPool = new List<KeyValuePair<string, string>>();

            //Dictionary<string, string> msgPool = new Dictionary<string, string>();
            db.Open();

            try
            {
                MySqlCommand command =
                    new MySqlCommand($"SELECT MESSAGE, senderID FROM {senderID + "_" + recepientID} ", db);
                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    msgPool.Add(KeyValuePair.Create(key: reader["senderID"].ToString(), value: reader["MESSAGE"].ToString()));
                }

                reader.Close();
            }
            catch (Exception e)
            {
                MySqlCommand command =
                    new MySqlCommand($"SELECT MESSAGE, senderID FROM {recepientID + "_" + senderID} ", db);
                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    msgPool.Add(KeyValuePair.Create(key: reader["senderID"].ToString(), value: reader["MESSAGE"].ToString()));
                }

                reader.Close();
            }

            db.Close();
            return msgPool;
        }


        public void Drop(string senderID, string recepientID)
        {
           
            db.Open();
            try
            {
                MySqlCommand command = new MySqlCommand($"DROP TABLE {senderID + "_" + recepientID}", db);
                command.ExecuteNonQuery();
               
            }
            catch (Exception e)
            {
                MySqlCommand command = new MySqlCommand($"DROP TABLE {recepientID + "_" + senderID}", db);
                command.ExecuteNonQuery();
              
            }

            db.Close();
           
        }
    }
}
