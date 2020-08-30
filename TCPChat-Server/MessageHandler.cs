using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using MessageDefs;

namespace TCPChat_Server
{
    class MessageHandler
    {
        // Keep a list of all current network streams to repeat incoming messages to.
        public static List<NetworkStream> NetStreams = new List<NetworkStream>();

        public static void RepeatToAllClients(string message, TcpClient client)
        {
            for (int i = 0; i < NetStreams.Count; i++)
            {
                ServerHandler.SendMessage(message, NetStreams[i]);
            }
        }

        private static string regexMatch(string source, string start, string end)
        {
            // Get the text in between two points.
            // This is a temporary fix until i get the json deserialization fixed.
            return source.Substring((source.IndexOf(start) + start.Length), (source.IndexOf(end) - source.IndexOf(start) - start.Length));

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

                    // Get the text between message and end chars.
                    string messageRegex = regexMatch(byteToString, "[{\"message\":\"", "\"}]");


                    string message = String.Format("{0} : {1}", ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString(), messageRegex);

                    Console.WriteLine(message);

                    RepeatToAllClients(message, client);

                    // Basically, the way I'm handling the incoming network stream
                    // Adds trailing bytes to each serialized message.
                    // Which means that it is very unreliable.
                    // Bad solution is just trimming the string to get the message. :(
                    /*try
                    {
                        List<MessageFormat> messageList = JsonSerializer.Deserialize<List<MessageFormat>>(bytesResized);

                        string message = String.Format("{0} : {1}", ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString(), messageList[0].message);
                        Console.WriteLine(message);

                        replyToAllClients(message, client);

                    }
                    catch (JsonException)
                    {
                        for (int ia = 0; ia < bytesResized.Length; ia++)
                        {
                            Console.WriteLine(bytesResized[ia]);
                        }

                    }*/




                }).Start();
            }
        }

    }
}
