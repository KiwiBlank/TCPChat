using CommonDefines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;

namespace TCPChat_Server
{

    class MessageHandler
    {
        // Keep a list of all current network streams to repeat incoming messages to.
        public static List<NetworkStream> NetStreams = new List<NetworkStream>();


        // TODO implement serialization for repeating to clients.
        public static void RepeatToAllClients(string serializedMessage, TcpClient client)
        {
            byte[] data = Encoding.ASCII.GetBytes(serializedMessage);
            for (int i = 0; i < NetStreams.Count; i++)
            {
                ServerHandler.SendMessage(data, NetStreams[i]);
            }
        }

        // The loop to recieve incoming packets.
        public static void RecieveMessage(NetworkStream stream, TcpClient client)
        {

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


                    string message = OutputMessage.ServerRecievedEncrypedMessage(bytesResized);

                    string messageFormatted = MessageSerialization.ReturnEndOfStreamString(message);
                    List<MessageFormat> messageList = Serialization.DeserializeMessageFormat(messageFormatted);

                    // Re-serialize to repeat for clients.
                    // TODO Implement Server Encryption for repeating messages.
                    // At the moment only the client encrypts its messages.

                    string repeatMessage = JsonSerializer.Serialize(messageList);

                    OutputMessage.ServerRecievedMessage(messageList);

                    RepeatToAllClients(repeatMessage, client);

                }).Start();
            }
        }

    }
}
