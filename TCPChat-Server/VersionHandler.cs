using CommonDefines;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace TCPChat_Server
{
    public class VersionHandler
    {
        public static bool VersionCheck(ClientInstance instance, string clientVersion)
        {
            // Check if versions do not match, if not close connection.
            if (clientVersion != Assembly.GetExecutingAssembly().GetName().Version.ToString())
            {
                string message = String.Format("Your version of: {0} does not match the server version of: {1}",
                    clientVersion,
                    Assembly.GetExecutingAssembly().GetName().Version.ToString());


                List<ServerMessageFormat> serverMessage = new List<ServerMessageFormat>();

                serverMessage.Add(new ServerMessageFormat
                {
                    MessageType = MessageTypes.SERVER,
                    Message = message,
                    Color = ConsoleColor.Yellow,
                    RSAExponent = Encryption.RSAExponent,
                    RSAModulus = Encryption.RSAModulus,
                });

                string json = Serialization.Serialize(serverMessage);

                byte[] data = Serialization.AddEndCharToMessage(json);

                int index = MessageHandler.FindClientKeysIndex(instance.client);

                byte[] encrypted = MessageHandler.EncryptMessage(data, ServerHandler.activeClients[index].RSAModulus, ServerHandler.activeClients[index].RSAExponent);

                StreamHandler.WriteToStream(instance.stream, encrypted);

                instance.client.Close();
                return false;
            }
            return true;
        }
    }
}
