using CommonDefines;
using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace TCPChat_Server
{
    public class ServerMessage
    {
        public static void ServerGlobalMessage(ConsoleColor color, string message)
        {
            List<ServerMessageFormat> serverMessage = new List<ServerMessageFormat>();

            serverMessage.Add(new ServerMessageFormat
            {
                MessageType = MessageTypes.SERVER,
                Message = message,
                Color = color,
                RSAExponent = Encryption.RSAExponent,
                RSAModulus = Encryption.RSAModulus,
            });
            ConsoleOutput.RecievedServerMessageFormat(serverMessage);
            MessageHandler.RepeatToAllClients(serverMessage);
        }
        public static void ServerClientMessage(ClientInstance instance, ConsoleColor color, string message)
        {
            List<ServerMessageFormat> serverMessage = new List<ServerMessageFormat>();

            serverMessage.Add(new ServerMessageFormat
            {
                MessageType = MessageTypes.SERVER,
                Message = message,
                Color = color,
                RSAExponent = Encryption.RSAExponent,
                RSAModulus = Encryption.RSAModulus,
            });

            string json = Serialization.Serialize(serverMessage, false);

            byte[] data = Serialization.AddEndCharToMessage(json);

            int index = FindClientKeysIndex(instance.client);

            byte[] encrypted = MessageHandler.EncryptMessage(data, ServerHandler.activeClients[index].RSAModulus, ServerHandler.activeClients[index].RSAExponent);

            StreamHandler.WriteToStream(instance.stream, encrypted);
        }
        public static int FindClientKeysIndex(TcpClient client)
        {
            for (int i = 0; i < ServerHandler.activeClients.Count; i++)
            {
                if (ServerHandler.activeClients[i].TCPClient == client)
                {
                    return i;
                }
            }
            return -1;
        }
    }
}
