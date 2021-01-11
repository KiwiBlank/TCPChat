using CommonDefines;
using System;
using System.Collections.Generic;

namespace TCPChat_Client
{
    public class CommandHandler
    {
        public static void RecievedDataReply(List<DataReplyFormat> list)
        {
            switch (list[0].DataType)
            {
                case CommandDataTypes.CLIENTLIST:
                    RecievedClientList(list[0].Data);
                    break;
                case CommandDataTypes.PING:
                    RecievedPingReply(list[0].Data);
                    break;
                case CommandDataTypes.CHANNELSWITCH:
                    RecievedNewChannelID(list[0].Data);
                    break;
                default:
                    break;
            }
        }
        public static void RecievedClientList(string data)
        {
            data = data.Replace(",", Environment.NewLine);
            Console.WriteLine("These users are currently online:");
            Console.WriteLine(data);
        }
        public static void RecievedPingReply(string data)
        {
            Console.WriteLine("Your latency to the server is: {0} ms", data);
        }
        public static void RecievedNewChannelID(string data)
        {
            if (int.Parse(data) == ClientRecievedTypes.CurrentChannelID)
            {
                Console.WriteLine("ERROR: Channel has not changed.");
                return;
            }
            ClientRecievedTypes.CurrentChannelID = int.Parse(data);
            Console.WriteLine("Channel switched to: {0}", data);
        }
    }
}
