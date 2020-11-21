using CommonDefines;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace TCPChat_Server
{

    class MessageHandler
    {

        public static void RepeatToAllClients(List<MessageFormat> list)
        {
            string json = Serialization.Serialize(list);

            byte[] data = Serialization.AddEndCharToMessage(json);

            for (int i = 0; i < ServerHandler.activeClients.Count; i++)
            {
                // Encrypt Message Data
                byte[] encrypt = Encryption.AESEncrypt(data, Encryption.AESKey, Encryption.AESIV);

                // Combine key values.
                RSAParameters publicKeyCombined = Encryption.RSAParamaterCombiner(ServerHandler.activeClients[i].RSAModulus, ServerHandler.activeClients[i].RSAExponent);

                // Encrypt Key Data
                byte[] finalBytes = Encryption.AppendKeyToMessage(encrypt, Encryption.AESKey, Encryption.AESIV, publicKeyCombined);

                // Try to send to [i] client, if the client does not exist anymore, remove from activeClients.
                try
                {
                    StreamHandler.WriteToStream(ServerHandler.activeClients[i].TCPClient.GetStream(), finalBytes);

                }
                catch (ObjectDisposedException)
                {
                    ServerHandler.activeClients.RemoveAt(i);
                }

            }
        }

        // The loop to recieve incoming packets.
        public static void RecieveMessage(ClientInstance instance)
        {

            byte[] bytes = new byte[8192];

            while ((instance.stream.Read(bytes, 0, bytes.Length)) > 0)
            {

                // I had some issues with trailing zero bytes, and this solves that.
                int i = bytes.Length - 1;
                while (bytes[i] == 0)
                {
                    --i;
                }

                byte[] bytesResized = new byte[i + 1];
                Array.Copy(bytes, bytesResized, i + 1);

                Array.Clear(bytes, 0, bytes.Length); // Seems to solve an issue where bytes from the last message stick around.



                // Is client verified, meaning client has established initial connection and communication.
                if (instance.clientVerified)
                {
                    ClientVerifiedRecieve(instance, bytesResized);
                }
                else
                {
                    ClientNotVerifiedRecieve(instance, bytesResized);
                }
            }
        }
        public static void ClientVerifiedRecieve(ClientInstance instance, byte[] bytes)
        {
            string message = Encryption.DecryptMessageData(bytes);

            string messageFormatted = MessageSerialization.ReturnEndOfStream(message);
            List<MessageFormat> messageList = Serialization.DeserializeMessageFormat(messageFormatted);

            OutputMessage.ServerRecievedMessage(messageList);

            // Encrypts the message and sends it to all clients.
            RepeatToAllClients(messageList);
        }
        public static void ClientNotVerifiedRecieve(ClientInstance instance, byte[] bytes)
        {
            string messageFormatted = MessageSerialization.ReturnEndOfStream(Encoding.ASCII.GetString(bytes));

            List<ConnectionMessageFormat> list = Serialization.DeserializeConnectionMessageFormat(messageFormatted);



            // Add this client to NetStreams to keep track of connection.
            ServerHandler.activeClients.Add(new ClientList
            {
                TCPClient = instance.client,
                RSAExponent = list[0].RSAExponent,
                RSAModulus = list[0].RSAModulus
            });

            instance.clientVerified = true;

            List<WelcomeMessageFormat> welcomeMessage = new List<WelcomeMessageFormat>();

            welcomeMessage.Add(new WelcomeMessageFormat
            {
                messageType = MessageTypes.WELCOME,
                connectMessage = ServerConfigFormat.serverChosenWelcomeMessage,
                serverName = ServerConfigFormat.serverChosenName,
                keyExponent = Encryption.RSAExponent,
                keyModulus = Encryption.RSAModulus,
                ServerVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString()
            });


            Serialize(welcomeMessage, instance);

            // Check if versions do not match, if not close connection.
            /*******  if (list[0].ClientVersion != Assembly.GetExecutingAssembly().GetName().Version.ToString())
              {
                  instance.client.Close();
              }*********************/
        }
        public static void Serialize<T>(List<T> message, ClientInstance instance)
        {
            string json = Serialization.Serialize(message);

            byte[] data = Serialization.AddEndCharToMessage(json);

            StreamHandler.WriteToStream(instance.stream, data);
        }
    }
}