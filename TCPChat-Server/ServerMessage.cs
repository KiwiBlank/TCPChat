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
            List<ServerMessageFormat> serverMessage = new();

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
        public static void ServerClientMessage(TcpClient client, ConsoleColor color, string message)
        {
            List<ServerMessageFormat> serverMessage = new();

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

            int index = FindClientKeysIndex(client);
            byte[] encrypted;

            if (ServerHandler.activeClients[index].EnableEncryption)
            {
                encrypted = MessageHandler.EncryptMessage(data, ServerHandler.activeClients[index].RSAModulus, ServerHandler.activeClients[index].RSAExponent);
            } else
            {
                encrypted = data;
            }

            StreamHandler.WriteToStream(client.GetStream(), encrypted);
        }
        public static void ClientPrivateMessage(TcpClient client, ConsoleColor color, string message, int userID, string username)
        { 
            List<PrivateMessageFormat> privateMessage = new();

            privateMessage.Add(new PrivateMessageFormat
            {
                MessageType = MessageTypes.PRIVATEMESSAGE,
                Message = message,
                Color = color,
                RSAExponent = Encryption.RSAExponent,
                RSAModulus = Encryption.RSAModulus,
                SenderUsername = username,
                SenderID = userID
            });

            string json = Serialization.Serialize(privateMessage, false);

            byte[] data = Serialization.AddEndCharToMessage(json);

            int index = FindClientKeysIndex(client);


            byte[] encrypted;

            if (ServerHandler.activeClients[index].EnableEncryption)
            {
                encrypted = MessageHandler.EncryptMessage(data, ServerHandler.activeClients[index].RSAModulus, ServerHandler.activeClients[index].RSAExponent);
            }
            else
            {
                encrypted = data;
            }

            StreamHandler.WriteToStream(client.GetStream(), encrypted);
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
