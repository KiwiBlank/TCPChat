﻿using MessageDefs;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
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
            for (int i = 0; i < NetStreams.Count; i++)
            {
                ServerHandler.SendMessage(serializedMessage, NetStreams[i]);
            }
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


                    string message = System.Text.Encoding.ASCII.GetString(bytesResized);

                    List<MessageFormat> messageList = MessageSerialization.DeserializeMessage(message);

                    OutputMessage.Output(messageList[0].message, messageList[0].IP, messageList[0].Username, messageList[0].UserNameColor);

                    RepeatToAllClients(message, client);

                }).Start();
            }
        }

    }
}
