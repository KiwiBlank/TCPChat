using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace CommonDefines
{
    public class Serialization
    {
        public static List<MessageFormat> DeserializeMessageFormat(string text)
        {
            List<MessageFormat> messageList = new List<MessageFormat>();

            messageList = JsonSerializer.Deserialize<List<MessageFormat>>(text);

            return messageList;
        }
        public static List<ConntectedMessageFormat> DeserializeConntectedMessageFormat(string text)
        {
            List<ConntectedMessageFormat> messageList = new List<ConntectedMessageFormat>();

            messageList = JsonSerializer.Deserialize<List<ConntectedMessageFormat>>(text);

            return messageList;
        }

        // Just to be used as a backup for if a list can't be identified correctly.
        public static List<MessageFormat> DeserializeDefault(string text)
        {
            List<MessageFormat> messageList = new List<MessageFormat>();

            messageList.Add(new MessageFormat { message = text, Username = null, UserNameColor = ConsoleColor.DarkGray, IP = null });

            return messageList;
        }
    }
}
