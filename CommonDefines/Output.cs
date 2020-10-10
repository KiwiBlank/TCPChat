using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace CommonDefines
{
    public class OutputMessage
    {
        public static void ServerRecievedMessage(List<MessageFormat> list)
        {
            OutputMessage.OutputMessageWithColor(list[0].message, list[0].IP, list[0].Username, list[0].UserNameColor);
        }

        public static void ClientRecievedMessageFormat(List<MessageFormat> list)
        {
            OutputMessage.OutputMessageWithColor(list[0].message, list[0].IP, list[0].Username, list[0].UserNameColor);
        }
        public static void ClientRecievedConnectedMessageFormat(List<WelcomeMessageFormat> list)
        {
            // Output Info
            Console.WriteLine(list[0].serverName);
            Console.WriteLine(list[0].connectMessage);

            // Encryption
            RSAParameters key = Encryption.RSAParamaterCombiner(list[0].keyModulus, list[0].keyExponent);
            Encryption.clientCopyOfServerPublicKey = key;

        }
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
