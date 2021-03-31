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
        public static void RecievedPrivateMessageFormat(List<PrivateMessageFormat> list)
        {
            CommonDefines.ConsoleOutput.OutputPrivateMessage(list[0].Message, list[0].Color, list[0].SenderUsername, list[0].SenderID);
        }
        public static void RecievedMessageReplyFormat(List<MessageReplyFormat> list, int channelID)
        {
            // CLIENT CAN ONLY RECIEVE MESSAGEREPLYFORMAT AS OF 1.3.0
            CommonDefines.ConsoleOutput.OutputMessage(list[0].Message, list[0].Username, list[0].UsernameColor, list[0].ID, channelID);
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
        public static void OutputMessage(string message, string username, ConsoleColor color, int id, int channelID)
        {
            Console.ForegroundColor = color;

            string output = String.Format("{0} - ({1}){2}: {3}", channelID, id, username, message);

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
        public static void OutputPrivateMessage(string message, ConsoleColor color, string senderUsername, int senderID)
        {
            Console.ForegroundColor = color;

            string output = String.Format("PRIVATE - ({0}){1}:{2}", senderID, senderUsername, message);

            Console.WriteLine(output);
            Console.ResetColor();
        }
    }
}
