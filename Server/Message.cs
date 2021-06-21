namespace Server
{
    /*public enum MessageType
    {
        date, sender, message
    }*/

    public enum TypeMessage
    {
        Stop, Message, Name
    }

    public class Message
    {
        public TypeMessage Type { get; set; }
        public int ID { get; set; }
        public string Msg { get; set; }
        public string SenderNickname { get; set; }
        public string ReceiverNickname { get; set; }
        public string Date { get; set; }
        public Message() { }

        public Message(string msg)
        {
            Msg = msg;
        }
    }
}