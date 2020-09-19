using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text.Json;

namespace MessageDefs
{
    [Serializable]

    public class ConntectedMessageFormat
    {
        public string connectMessage { get; set; }

        public string serverName { get; set; }

        public RSAParameters publicKey { get; set; }
    }
    [Serializable]
    // Inherits the userconfigformat as of now.
    public class MessageFormat : UserConfigFormat
    {
        public string message { get; set; }

        public string IP { get; set; }
    }
    [Serializable]
    // The format that the user config should follow.
    public class UserConfigFormat
    {
        public string Username { get; set; }
        public ConsoleColor UserNameColor { get; set; }
    }
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
    public class MessageSerialization
    {

        // This method is supposed to figure out what class the serialized message belongs to.
        // When Deserializing, you can output it to any list class regardless if it has the correct members.
        // The incorrect values will be null when deserializing.
        public static int FindTypeOfList(string message)
        {

            List<MessageFormat> messageFormatList = new List<MessageFormat>();
            messageFormatList = JsonSerializer.Deserialize<List<MessageFormat>>(message);

            // Has to be constant value that can never be null for this check to work.
            if (messageFormatList[0].message != null)
            {
                return 1;
            }

            List<ConntectedMessageFormat> conntectedMessageFormatList = new List<ConntectedMessageFormat>();
            conntectedMessageFormatList = JsonSerializer.Deserialize<List<ConntectedMessageFormat>>(message);

            // Has to be constant value that can never be null for this check to work.
            if (conntectedMessageFormatList[0].connectMessage != null)
            {
                return 2;
            }
            return 0;

        }
        public static string ReturnEndOfStreamString(string text)
        {
            int indexToRemove = FindEndOfStream(text.ToCharArray());
            if (indexToRemove != 0)
            {
                text = text.Remove(indexToRemove);

            }
            return text;
        }

        public static List<MessageFormat> DeserializeMessageFormat (string text)
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
        // Find the end of stream, to then trim trailing characters.
        public static int FindEndOfStream(char[] arr)
        {

            for (int i = 0; i < arr.Length; i++)
            {
                try
                {
                    // A really bad way to find the end of stream.
                    // This is to return the point where all trailing bytes should be removed.
                    if (arr[i] == '}' && arr[i + 1] == ']' && arr[i + 2] == '')
                    {
                        return i + 2;
                    }
                }
                catch (IndexOutOfRangeException)
                {
                }

            }
            return 0;
        }

    }

}
