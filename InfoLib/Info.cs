using System;

namespace InfoLib
{
    static public class Info
    {
        static public void ShowInfo(string message)
        {
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        static public void ShowLog(string sender, string date, string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Клиент {sender} at {date}: {message}.");
            Console.ResetColor();
        }

        static public void ShowMessage(string sender, string receiver, string date, string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{sender} to {receiver} at {date}: {message}.");
            Console.ResetColor();
        }
    }
}
