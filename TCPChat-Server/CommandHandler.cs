using CommonDefines;
using System;
using System.Collections.Generic;
using System.Text;

namespace TCPChat_Server
{
    public class CommandHandler
    {
        // The server has recieved an information request from a client.
        public static void RecievedDataRequest(ClientInstance instance, List<DataRequestFormat> list)
        {
            switch (list[0].DataType)
            {
                case CommandDataTypes.CLIENTLIST:
                    string clients = ClientListString();

                    ReplyToDataRequest(instance, clients, CommandDataTypes.CLIENTLIST);
                    break;
                default:
                    break;
            }
        }
        public static void ReplyToDataRequest(ClientInstance instance, string message, CommandDataTypes dataType)
        {
            List<DataReplyFormat> serverMessage = new List<DataReplyFormat>();

            serverMessage.Add(new DataReplyFormat
            {
                MessageType = MessageTypes.DATAREPLY,
                DataType = dataType,
                Data = message,
            });

            string json = Serialization.Serialize(serverMessage);

            byte[] data = Serialization.AddEndCharToMessage(json);

            int index = ServerMessage.FindClientKeysIndex(instance.client);

            byte[] encrypted = MessageHandler.EncryptMessage(data, ServerHandler.activeClients[index].RSAModulus, ServerHandler.activeClients[index].RSAExponent);

            StreamHandler.WriteToStream(instance.stream, encrypted);
        }
        public static string ClientListString()
        {
            List<string> usernames = new List<string>();

            for (int i = 0; i < ServerHandler.activeClients.Count; i++)
            {
                usernames.Add(ServerHandler.activeClients[i].Username);
            }

            string finalList = String.Join(",", usernames);

            return finalList;

        }
    }
}
