using CommonDefines;
using System;
using System.Collections.Generic;

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
                case CommandDataTypes.PING:
                    string replyTime = PingReply(list[0].Parameters);
                    ReplyToDataRequest(instance, replyTime, CommandDataTypes.PING);
                    break;
                case CommandDataTypes.CHANNELSWITCH:
                    ClientChannelSwitch(instance, list[0].Parameters);
                    // Replies in method inside ChannelHandler
                    break;
                case CommandDataTypes.PRIVATEMESSSAGE:
                    ClientPrivateMessage(instance, list[0].Parameters);
                    // Replies in method inside ChannelHandler
                    break;
                default:
                    break;
            }
        }
        public static void ReplyToDataRequest(ClientInstance instance, string message, CommandDataTypes dataType)
        {
            List<DataReplyFormat> serverMessage = new();

            serverMessage.Add(new DataReplyFormat
            {
                MessageType = MessageTypes.DATAREPLY,
                DataType = dataType,
                Data = message,
            });

            string json = Serialization.Serialize(serverMessage, false);

            byte[] data = Serialization.AddEndCharToMessage(json);

            int index = ServerMessage.FindClientKeysIndex(instance.client);


            if (ServerHandler.activeClients[index].EnableEncryption)
            {
                byte[] encrypted = MessageHandler.EncryptMessage(data, ServerHandler.activeClients[index].RSAModulus, ServerHandler.activeClients[index].RSAExponent);
                StreamHandler.WriteToStream(instance.stream, encrypted);
            }
            else
            {
                StreamHandler.WriteToStream(instance.stream, data);
            }


        }
        public static string ClientListString()
        {
            List<string> usernames = new();

            for (int i = 0; i < ServerHandler.activeClients.Count; i++)
            {
                usernames.Add(ServerHandler.activeClients[i].Username);
            }

            string finalList = String.Join(",", usernames);

            return finalList;
        }
        public static string PingReply(string pingTime)
        {
            long pingParse = long.Parse(pingTime);
            long pongTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            long timeDifference = pongTime - pingParse;

            return timeDifference.ToString();
        }
        public static void ClientChannelSwitch(ClientInstance instance, string parameters)
        {
            ChannelHandler.ClientRequestsChannelSwitch(instance, parameters);
        }
        public static void ClientPrivateMessage(ClientInstance instance, string parameters)
        {
            string[] args = parameters.Split(',');

            for (int i = 0; i < ServerHandler.activeClients.Count; i++)
            {
                if (args[0] == ServerHandler.activeClients[i].ID.ToString() && !String.IsNullOrEmpty(args[1]))
                {
                    // Get the index of the client sending the message.
                    int index = ServerMessage.FindClientKeysIndex(instance.client);

                    // Send the message to the reciever.
                    ServerMessage.ClientPrivateMessage(
                        ServerHandler.activeClients[i].TCPClient,
                        ConsoleColor.Yellow,
                        args[1],
                        ServerHandler.activeClients[index].ID,
                        ServerHandler.activeClients[index].Username
                        );

                    // Send a message to the sender to confirm the message has been sent.
                    ServerMessage.ServerClientMessage(instance.client, ConsoleColor.Yellow, "Message has been sent.");
                }
            }

        }
    }
}
