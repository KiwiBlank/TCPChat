using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace CommonDefines
{
    public class Serialization
    {
        public static List<DataReplyFormat> DeserializeDataReplyFormat(string text)
        {
            List<DataReplyFormat> messageList = JsonSerializer.Deserialize<List<DataReplyFormat>>(text);

            return messageList;
        }
        public static List<DataRequestFormat> DeserializeDataRequestFormat(string text)
        {
            List<DataRequestFormat> messageList = JsonSerializer.Deserialize<List<DataRequestFormat>>(text);

            return messageList;
        }
        public static List<ServerMessageFormat> DeserializeServerMessageFormat(string text)
        {
            List<ServerMessageFormat> messageList = JsonSerializer.Deserialize<List<ServerMessageFormat>>(text);

            return messageList;
        }
        public static List<ConnectionMessageFormat> DeserializeConnectionMessageFormat(string text)
        {
            List<ConnectionMessageFormat> messageList = JsonSerializer.Deserialize<List<ConnectionMessageFormat>>(text);

            return messageList;
        }
        public static List<MessageFormat> DeserializeMessageFormat(string text)
        {
            List<MessageFormat> messageList = JsonSerializer.Deserialize<List<MessageFormat>>(text);

            return messageList;
        }
        public static List<MessageReplyFormat> DeserializeMessageReplyFormat(string text)
        {
            List<MessageReplyFormat> messageList = JsonSerializer.Deserialize<List<MessageReplyFormat>>(text);

            return messageList;
        }
        public static List<WelcomeMessageFormat> DeserializeWelcomeMessageFormat(string text)
        {
            List<WelcomeMessageFormat> messageList = JsonSerializer.Deserialize<List<WelcomeMessageFormat>>(text);

            return messageList;
        }

        public static string Serialize<T>(List<T> list, bool indent)
        {
            string json;
            if (indent)
            {
                json = JsonSerializer.Serialize(list, new JsonSerializerOptions { WriteIndented = true });
            }
            else
            {
                json = JsonSerializer.Serialize(list);
            }

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
