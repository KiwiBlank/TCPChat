using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading;
using MessageDefs;

namespace TCPChat_Server
{

    class MessageHandler
    {
        // Keep a list of all current network streams to repeat incoming messages to.
        public static List<NetworkStream> NetStreams = new List<NetworkStream>();


        // TODO implement serialization for repeating to clients.
        public static void RepeatToAllClients(string message, TcpClient client, ConsoleColor color)
        {
            for (int i = 0; i < NetStreams.Count; i++)
            {
                ServerHandler.SendMessage(message, NetStreams[i], color);
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
        public static void DeserializeText(string text, TcpClient client)
        {
            // More than likely don't need a new thread
            new Thread(() =>
            {

                int indexToRemove = FindEndOfStream(text.ToCharArray());
                if (indexToRemove != 0)
                {
                    text = text.Remove(indexToRemove);

                }
                List<MessageFormat> messageList = new List<MessageFormat>();
                messageList = JsonSerializer.Deserialize<List<MessageFormat>>(text);
                
                string message = String.Format("{0} : {1}", ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString(), messageList[0].message);

                Console.ForegroundColor = messageList[0].UserNameColor;
                Console.WriteLine(message);
                Console.ResetColor();

                RepeatToAllClients(message, client, messageList[0].UserNameColor);
                messageList.Clear();


            }).Start();

        }

        // The loop to recieve incoming packets.
        public static void RecieveMessage(NetworkStream stream, TcpClient client)
        {
            NetStreams.Add(stream);

            byte[] bytes = new byte[8192];

            int iStream;

            while ((iStream = stream.Read(bytes, 0, bytes.Length)) > 0)
            {
                new Thread(() =>
                {
                    // I had some issues with trailing zero bytes, and this solves that.
                    int i = bytes.Length - 1;
                    while (bytes[i] == 0)
                    {
                        --i;
                    }

                    byte[] bytesResized = new byte[i + 1];
                    Array.Copy(bytes, bytesResized, i + 1);


                    string byteToString = System.Text.Encoding.ASCII.GetString(bytesResized);

                    DeserializeText(byteToString, client);


                }).Start();
            }
        }

    }
}
