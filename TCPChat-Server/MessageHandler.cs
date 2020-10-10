using CommonDefines;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace TCPChat_Server
{

    class MessageHandler
    {

        // TODO implement serialization for repeating to clients.
        public static void RepeatToAllClients(List<MessageFormat> list)
        {
            string json = Serialization.Serialize(list);

            byte[] data = Serialization.AddEndCharToMessage(json);

            for (int i = 0; i < ServerHandler.activeClients.Count; i++)
            {
                // Encrypt Message Data
                byte[] encrypt = Encryption.AESEncrypt(data, Encryption.AESKey, Encryption.AESIV);

                RSAParameters publicKeyCombined = Encryption.RSAParamaterCombiner(ServerHandler.activeClients[i].RSAModulus, ServerHandler.activeClients[i].RSAExponent);

                // Encrypt Key Data
                byte[] finalBytes = Encryption.AppendKeyToMessage(encrypt, Encryption.AESKey, Encryption.AESIV, publicKeyCombined);

                try
                {
                    ServerHandler.SendMessage(finalBytes, ServerHandler.activeClients[i].TCPClient.GetStream());
                }
                catch (ObjectDisposedException)
                {

                    ServerHandler.activeClients.RemoveAt(i);

                }

            }
        }

        // The loop to recieve incoming packets.
        public static void RecieveMessage(NetworkStream stream, TcpClient client)
        {
            // Client verified means that the client has sent over its encryption keys, and therefore can send encrypted messages.
            bool clientVerified = false;

            byte[] bytes = new byte[8192];

            while (!stream.DataAvailable)
            {
                Thread.Sleep(100);
            }

            while ((stream.Read(bytes, 0, bytes.Length)) > 0)
            {

                // I had some issues with trailing zero bytes, and this solves that.
                int i = bytes.Length - 1;
                while (bytes[i] == 0)
                {
                    --i;
                }

                byte[] bytesResized = new byte[i + 1];
                Array.Copy(bytes, bytesResized, i + 1);

                stream.Flush(); // May not do anything.
                Array.Clear(bytes, 0, bytes.Length); // Seems to solve an issue where bytes from the last message stick around.

                if (clientVerified)
                {

                    string message = Encryption.DecryptMessageData(bytesResized);

                    string messageFormatted = MessageSerialization.ReturnEndOfStreamString(message);
                    List<MessageFormat> messageList = Serialization.DeserializeMessageFormat(messageFormatted);

                    // Re-serialize to repeat for clients.
                    // TODO Implement Server Encryption for repeating messages.
                    // At the moment only the client encrypts its messages.
                    OutputMessage.ServerRecievedMessage(messageList);

                    RepeatToAllClients(messageList);
                }
                else
                {
                    string message = Encoding.ASCII.GetString(bytesResized);

                    string messageFormatted = MessageSerialization.ReturnEndOfStreamString(message);

                    List<ConnectionMessageFormat> list = Serialization.DeserializeConnectionMessageFormat(messageFormatted);

                    // Add this client to NetStreams to keep track of connection.
                    ServerHandler.activeClients.Add(new ClientList
                    {
                        TCPClient = client,
                        RSAExponent = list[0].RSAExponent,
                        RSAModulus = list[0].RSAModulus
                    });

                    clientVerified = true;
                }
            }
        }

    }
}
