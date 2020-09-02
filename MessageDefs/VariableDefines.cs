using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading;

namespace MessageDefs
{
    [Serializable]
    // Inherits the userconfigformat as of now.
    public class MessageFormat : UserConfigFormat
    {
        public string message { get; set; }

        public string IP { get; set; }
    }
    // TODO Implement this to better identify users.
    public class UserInfo
    {
        public string name { get; set; }

        public int id { get; set; }

        public EndPoint ip { get; set; }

        public TcpClient userInfoclient { get; set; }

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
        // This is the common output method for both server and client.
        public static void Output (string message, string IP, string username, ConsoleColor color)
        {

            // If IP or username is empty
            // This would indicate the server has sent a message which does not have that info.
            // TODO Better Solution
            if (String.IsNullOrEmpty(IP) || String.IsNullOrEmpty(username))
            {
                string output = String.Format("{0}", message);

                Console.WriteLine(output);
                Console.ResetColor();
            } else
            {
                Console.ForegroundColor = color;
                string output = String.Format("{0} : {1} - {2}", IP, username, message);

                Console.WriteLine(output);
                Console.ResetColor();
            }

        }
    }
    public class MessageSerialization
    {
        public static List<MessageFormat> DeserializeMessage(string text)
        {
            int indexToRemove = FindEndOfStream(text.ToCharArray());
            if (indexToRemove != 0)
            {
                text = text.Remove(indexToRemove);
            
            }

            // If the message is corrupt or not in json format, just output as a pure string.
            try
            {

                List<MessageFormat> messageList = new List<MessageFormat>();

                messageList = JsonSerializer.Deserialize<List<MessageFormat>>(text);

                return messageList;

            }
            catch (JsonException)
            {
                List<MessageFormat> catchReturn = new List<MessageFormat>();
                // See the messageformat class in VariableDefines.
                // Userchosen variables are defined in confighandler.
                catchReturn.Add(new MessageFormat { message = text, Username = null, UserNameColor = ConsoleColor.DarkGray, IP = null });

                return catchReturn;
            }
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
