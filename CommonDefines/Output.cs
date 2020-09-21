using System;

namespace CommonDefines
{
    public class OutputMessage
    {
        public static void OutputOnlyMessage(string message)
        {

            Console.WriteLine(message);
            Console.ResetColor();

        }
        // This is the common output method for both server and client.
        public static void OutputMessageWithColor(string message, string IP, string username, ConsoleColor color)
        {

            Console.ForegroundColor = color;
            string output = String.Format("{0} : {1} - {2}", IP, username, message);

            Console.WriteLine(output);
            Console.ResetColor();

        }
    }
}
