using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace CommonDefines
{
    public class OutputMessage
    {
        public static void ServerRecievedMessage(List<MessageFormat> list)
        {
            OutputMessage.OutputMessageColor(list[0].message, list[0].Username, list[0].UserNameColor);
        }
        public static void ClientRecievedServerMessageFormat(List<ServerMessageFormat> list)
        {
            OutputMessage.OutputServerMessage(list[0].message, list[0].color);
        }
        public static void ClientRecievedMessageFormat(List<MessageFormat> list)
        {
            OutputMessage.OutputMessageColor(list[0].message, list[0].Username, list[0].UserNameColor);
        }
        public static void ClientRecievedWelcomeMessageFormat(List<WelcomeMessageFormat> list)
        {
            // Output Info
            Console.WriteLine(list[0].serverName);
            Console.WriteLine(list[0].connectMessage);

            // Encryption
            RSAParameters key = Encryption.RSAParamaterCombiner(list[0].keyModulus, list[0].keyExponent);
            Encryption.clientCopyOfServerPublicKey = key;
        }
        // This is the common output method for both server and client.
        public static void OutputMessageColor(string message, string username, ConsoleColor color)
        {
            Console.ForegroundColor = color;

            string output = String.Format("{0}: {1}", username, message);

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
