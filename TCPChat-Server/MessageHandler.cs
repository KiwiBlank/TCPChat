using CommonDefines;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace TCPChat_Server
{

    class MessageHandler
    {
        public static byte[] EncryptMessage(byte[] data, byte[] modulus, byte[] exponent)
        {
            // Encrypt Message Data
            byte[] encrypt = Encryption.AESEncrypt(data, Encryption.AESKey, Encryption.AESIV);

            // Combine key values.
            RSAParameters publicKeyCombined = Encryption.RSAParamaterCombiner(modulus, exponent);

            // Encrypt Key Data
            byte[] finalBytes = Encryption.AppendKeyToMessage(encrypt, Encryption.AESKey, Encryption.AESIV, publicKeyCombined);

            return finalBytes;
        }
        public static void RepeatToAllClients<T>(List<T> list)
        {
            string json = Serialization.Serialize(list, false);

            byte[] data = Serialization.AddEndCharToMessage(json);

            for (int i = 0; i < ServerHandler.activeClients.Count; i++)
            {
                byte[] encrypted = EncryptMessage(data, ServerHandler.activeClients[i].RSAModulus, ServerHandler.activeClients[i].RSAExponent);
                // Try to send to [i] client, if the client does not exist anymore, remove from activeClients.
                try
                {
                    StreamHandler.WriteToStream(ServerHandler.activeClients[i].TCPClient.GetStream(), encrypted);

                }
                catch (ObjectDisposedException)
                {
                    ServerHandler.activeClients.RemoveAt(i);
                }
                catch (InvalidOperationException)
                {
                    ServerHandler.activeClients.RemoveAt(i);
                }

            }
        }


        // The loop to recieve incoming packets.
        public static void RecieveMessage(ClientInstance instance)
        {
            long lastMessage = 0;

            byte[] bytes = new byte[8192];

            while ((instance.stream.Read(bytes, 0, bytes.Length)) > 0)
            {
                // Checks if the client has sent messages too fast.
                if (DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - lastMessage >= ServerConfigFormat.serverChosenClientTime)
                {
                    lastMessage = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

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
                        VerifiedRecieve(instance, bytesResized);
                    }
                    else
                    {
                        NotVerifiedRecieve(instance, bytesResized);
                    }
                }
            }
        }
        public static void VerifiedRecieve(ClientInstance instance, byte[] bytes)
        {
            // Random CryptographicException that i am not able to replicate
            // TODO Investigate reason for exception.
            string message = "";
            try
            {
                message = Encryption.DecryptMessageData(bytes);

            }
            catch (CryptographicException)
            {

                int index = ServerMessage.FindClientKeysIndex(instance.client);
                string serverMessage = String.Format("{0} Was kicked due to a cryptography error.", ServerHandler.activeClients[index].Username);
                ServerMessage.ServerGlobalMessage(ConsoleColor.Yellow, serverMessage);
                instance.client.Close();
            }

            string messageFormatted = Common.ReturnEndOfStream(message);

            byte[] messageBytes = Encoding.ASCII.GetBytes(messageFormatted);

            List<MessageFormat> messageList;

            switch (Common.ReturnMessageType(messageBytes))
            {
                case MessageTypes.MESSAGE:
                    // Error when deserializing message.
                    // Could mean corrupt or icorrect data has been transmitted.

                    try
                    {
                        messageList = Serialization.DeserializeMessageFormat(messageFormatted);
                        ConsoleOutput.RecievedMessageFormat(messageList);

                        // Encrypts the message and sends it to all clients.
                        RepeatToAllClients(messageList);
                    }
                    catch (JsonException)
                    {
                        int index = ServerMessage.FindClientKeysIndex(instance.client);
                        string serverMessage = String.Format("{0} Was kicked due to an invalid message.", ServerHandler.activeClients[index].Username);
                        ServerMessage.ServerGlobalMessage(ConsoleColor.Yellow, serverMessage);
                        instance.client.Close();
                    }
                    break;
                case MessageTypes.DATAREQUEST:
                    List<DataRequestFormat> dataList = Serialization.DeserializeDataRequestFormat(messageFormatted);
                    CommandHandler.RecievedDataRequest(instance, dataList);
                    break;
            }
        }
        public static void NotVerifiedRecieve(ClientInstance instance, byte[] bytes)
        {
            string messageFormatted = Common.ReturnEndOfStream(Encoding.ASCII.GetString(bytes));

            List<ConnectionMessageFormat> list = Serialization.DeserializeConnectionMessageFormat(messageFormatted);


            // Add this client to NetStreams to keep track of connection.
            ServerHandler.activeClients.Add(new ClientList
            {
                TCPClient = instance.client,
                Username = list[0].Username,
                RSAExponent = list[0].RSAExponent,
                RSAModulus = list[0].RSAModulus
            });

            // Check if server and client versions are the same before continuing.
            /*if (!VersionHandler.VersionCheck(instance, list[0].ClientVersion))
            {
                // Remove the item just added to active clients.
                // The reason it is added before is to have a list to index when sending server message to.
                ServerHandler.activeClients.RemoveAt(ServerHandler.activeClients.Count - 1);
                return;
            }*/

            string message = String.Format("{0} has connected.", list[0].Username);
            ServerMessage.ServerGlobalMessage(ConsoleColor.Yellow, message);


            instance.clientVerified = true;

            List<WelcomeMessageFormat> welcomeMessage = new List<WelcomeMessageFormat>();

            welcomeMessage.Add(new WelcomeMessageFormat
            {
                MessageType = MessageTypes.WELCOME,
                ConnectMessage = ServerConfigFormat.serverChosenWelcomeMessage,
                ServerName = ServerConfigFormat.serverChosenName,
                RSAExponent = Encryption.RSAExponent,
                RSAModulus = Encryption.RSAModulus,
                ServerVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString()
            });


            Serialize(welcomeMessage, instance, false);
        }
        public static void Serialize<T>(List<T> message, ClientInstance instance, bool encrypt)
        {
            string json = Serialization.Serialize(message, false);

            byte[] data = Serialization.AddEndCharToMessage(json);

            if (encrypt)
            {
                int index = ServerMessage.FindClientKeysIndex(instance.client);
                data = EncryptMessage(data, ServerHandler.activeClients[index].RSAModulus, ServerHandler.activeClients[index].RSAExponent);
            }

            StreamHandler.WriteToStream(instance.stream, data);
        }
    }
}