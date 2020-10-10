using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.Json;

namespace CommonDefines
{
    public class Serialization
    {

        public static List<ConnectionMessageFormat> DeserializeConnectionMessageFormat(string text)
        {
            List<ConnectionMessageFormat> messageList = new List<ConnectionMessageFormat>();

            messageList = JsonSerializer.Deserialize<List<ConnectionMessageFormat>>(text);

            return messageList;
        }
        public static List<MessageFormat> DeserializeMessageFormat(string text)
        {
            List<MessageFormat> messageList = new List<MessageFormat>();

            messageList = JsonSerializer.Deserialize<List<MessageFormat>>(text);

            return messageList;
        }
        public static List<WelcomeMessageFormat> DeserializeWelcomeMessageFormat(string text)
        {
            List<WelcomeMessageFormat> messageList = new List<WelcomeMessageFormat>();

            messageList = JsonSerializer.Deserialize<List<WelcomeMessageFormat>>(text);

            return messageList;
        }

        // Just to be used as a backup for if a list can't be identified correctly.
        public static List<MessageFormat> DeserializeDefault(string text)
        {
            List<MessageFormat> messageList = new List<MessageFormat>();

            messageList.Add(new MessageFormat { message = text, Username = null, UserNameColor = ConsoleColor.DarkGray, IP = null });

            return messageList;
        }
        public static string Serialize <T>(List<T> list)
        {
            string json = JsonSerializer.Serialize(list);

            return json;
        }
        public static byte[] AddEndCharToMessage(string message)
        {
            Byte[] data = Encoding.ASCII.GetBytes(message);
            List<Byte> byteToList = data.ToList();

            byteToList.Add(0x01); // Add end char

            Byte[] dataToArray = byteToList.ToArray();
            return dataToArray;
        }
    }
}
