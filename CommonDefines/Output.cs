using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace CommonDefines
{
    public class ConsoleOutput
    {
        public static void RecievedServerMessageFormat(List<ServerMessageFormat> list)
        {
            CommonDefines.ConsoleOutput.OutputServerMessage(list[0].Message, list[0].Color);
        }
        public static void RecievedMessageFormat(List<MessageFormat> list)
        {
            // TODO Include user's current channel in output.
            CommonDefines.ConsoleOutput.OutputMessage(list[0].Message, list[0].Username, list[0].UserNameColor, list[0].ID);
        }
        public static void RecievedWelcomeMessageFormat(List<WelcomeMessageFormat> list)
        {
            // Output Info
            Console.WriteLine(list[0].ServerName);
            Console.WriteLine(list[0].ConnectMessage);

            // Encryption
            RSAParameters key = Encryption.RSAParamaterCombiner(list[0].RSAModulus, list[0].RSAExponent);
            Encryption.clientCopyOfServerPublicKey = key;
        }
        // This is the common output method for both server and client.
        public static void OutputMessage(string message, string username, ConsoleColor color, int id)
        {
            Console.ForegroundColor = color;

            string output = String.Format("({0}) {1}: {2}", id, username, message);

            Console.WriteLine(output);
            Console.ResetColor();
        }
        public static void OutputServerMessage(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;

            string output = String.Format("SERVER - {0}", message);

            Console.WriteLine(output);
            Console.ResetColor();
        }
    }
}
